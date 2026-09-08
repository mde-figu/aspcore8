// EcoTracker NoSQL — READ: consultas (find, projeção, operadores e agregações)
db = db.getSiblingDB("ecotracker");
const sep = t => print("\n=============== " + t + " ===============");

sep("R1 organizacoes: setor Indústria/Química com score < 90 (projeção)");
printjson(db.organizacoes.find(
  { setor: { $in: ["Indústria", "Química"] }, scoreConformidade: { $lt: 90 } },
  { _id: 0, razaoSocial: 1, setor: 1, scoreConformidade: 1 }
).sort({ scoreConformidade: -1 }).toArray());

sep("R2 organizacoes: documento completo (esquema flexível — subdocumentos e arrays)");
printjson(db.organizacoes.findOne({ cnpj: "12.345.678/0001-90" }, { _id: 0 }));

sep("R3 emissoes_carbono: emissões de escopo 1 acima de 1000 tCO2e");
printjson(db.emissoes_carbono.find(
  { escopo: 1, toneladasCO2e: { $gt: 1000 } },
  { _id: 0, cnpj: 1, competencia: 1, fonte: 1, toneladasCO2e: 1 }
).sort({ toneladasCO2e: -1 }).toArray());

sep("R4 emissoes_carbono: total de tCO2e por organização e escopo (aggregate)");
printjson(db.emissoes_carbono.aggregate([
  { $group: { _id: { cnpj: "$cnpj", escopo: "$escopo" }, totalTonCO2e: { $sum: "$toneladasCO2e" }, lancamentos: { $sum: 1 } } },
  { $sort: { totalTonCO2e: -1 } },
  { $limit: 6 }
]).toArray());

sep("R5 emissoes_carbono + organizacoes: $lookup (join) das maiores emissoras");
printjson(db.emissoes_carbono.aggregate([
  { $group: { _id: "$cnpj", totalTonCO2e: { $sum: "$toneladasCO2e" } } },
  { $lookup: { from: "organizacoes", localField: "_id", foreignField: "cnpj", as: "org" } },
  { $project: { _id: 0, cnpj: "$_id", totalTonCO2e: 1, razaoSocial: { $first: "$org.razaoSocial" },
                setor: { $first: "$org.setor" } } },
  { $sort: { totalTonCO2e: -1 } }, { $limit: 5 }
]).toArray());

sep("R6 licencas_ambientais: vencendo nos próximos 120 dias a partir de 01/08/2026");
printjson(db.licencas_ambientais.find(
  { validade: { $gte: ISODate("2026-08-01"), $lte: ISODate("2026-11-29") } },
  { _id: 0, numero: 1, tipo: 1, orgao: 1, validade: 1, situacao: 1 }
).sort({ validade: 1 }).toArray());

sep("R7 licencas_ambientais: condicionantes ainda não atendidas ($elemMatch)");
printjson(db.licencas_ambientais.find(
  { condicionantes: { $elemMatch: { atendida: false } } },
  { _id: 0, numero: 1, "condicionantes.descricao": 1, "condicionantes.prazo": 1 }
).toArray());

sep("R8 auditorias_conformidade: auditorias não conformes e score médio por norma");
printjson(db.auditorias_conformidade.find({ resultado: "Não conforme" }, { _id: 0, cnpj: 1, norma: 1, scoreObtido: 1 }).toArray());
printjson(db.auditorias_conformidade.aggregate([
  { $group: { _id: "$norma", scoreMedio: { $avg: "$scoreObtido" }, auditorias: { $sum: 1 } } },
  { $sort: { scoreMedio: -1 } }
]).toArray());

sep("R9 auditorias_conformidade: itens sociais reprovados ($unwind + $match)");
printjson(db.auditorias_conformidade.aggregate([
  { $unwind: "$itens" },
  { $match: { "itens.dimensao": "Social", "itens.conforme": false } },
  { $project: { _id: 0, cnpj: 1, norma: 1, requisito: "$itens.requisito" } }
]).toArray());

sep("R10 alertas_ambientais: alertas abertos por severidade");
printjson(db.alertas_ambientais.aggregate([
  { $match: { resolvido: false } },
  { $group: { _id: "$severidade", quantidade: { $sum: 1 }, tipos: { $addToSet: "$tipo" } } },
  { $sort: { quantidade: -1 } }
]).toArray());

sep("R11 alertas_ambientais: payloads diferentes na mesma collection");
printjson(db.alertas_ambientais.find(
  { tipo: { $in: ["QUALIDADE_AR", "LIMITE_EMISSAO", "CONSUMO_ENERGIA"] } },
  { _id: 0, tipo: 1, severidade: 1, dados: 1 }
).limit(4).toArray());

sep("R12 índices criados em cada collection");
["organizacoes", "emissoes_carbono", "licencas_ambientais", "auditorias_conformidade", "alertas_ambientais"]
  .forEach(c => print(c + ": " + db[c].getIndexes().map(i => i.name).join(", ")));
