// EcoTracker NoSQL — UPDATE: atualizações simples, em lote, em arrays e upsert
db = db.getSiblingDB("ecotracker");
const sep = t => print("\n=============== " + t + " ===============");

sep("U1 organizacoes: atualizar score de conformidade após auditoria (updateOne + $set/$currentDate)");
printjson(db.organizacoes.updateOne(
  { cnpj: "67.890.123/0001-45" },
  { $set: { scoreConformidade: 41.2, riscoAmbiental: "Crítico" }, $currentDate: { atualizadoEm: true } }
));
printjson(db.organizacoes.findOne({ cnpj: "67.890.123/0001-45" }, { _id: 0, razaoSocial: 1, scoreConformidade: 1, riscoAmbiental: 1, atualizadoEm: 1 }));

sep("U2 organizacoes: adicionar certificação sem duplicar ($addToSet) — esquema flexível");
printjson(db.organizacoes.updateOne(
  { cnpj: "56.789.012/0001-34" },
  { $addToSet: { certificacoes: "ISO 14064-1" }, $set: { "indicadoresSociais.pcd": 0.07 } }
));
printjson(db.organizacoes.findOne({ cnpj: "56.789.012/0001-34" }, { _id: 0, razaoSocial: 1, certificacoes: 1, indicadoresSociais: 1 }));

sep("U3 emissoes_carbono: registrar compensação de carbono e reduzir o saldo ($inc)");
printjson(db.emissoes_carbono.updateOne(
  { cnpj: "12.345.678/0001-90", competencia: "2026-01", escopo: 1 },
  { $inc: { toneladasCO2e: -50 },
    $set: { compensacao: { creditos: 50, projeto: "Reflorestamento Mata Atlântica", certificadora: "Verra", data: ISODate("2026-08-20") } } }
));
printjson(db.emissoes_carbono.findOne({ cnpj: "12.345.678/0001-90", competencia: "2026-01", escopo: 1 }, { _id: 0, toneladasCO2e: 1, compensacao: 1 }));

sep("U4 emissoes_carbono: marcar em lote os lançamentos acima do limite (updateMany)");
printjson(db.emissoes_carbono.updateMany(
  { toneladasCO2e: { $gt: 1000 } },
  { $set: { classificacao: "ALTO_IMPACTO", revisarInventario: true } }
));
printjson(db.emissoes_carbono.find({ classificacao: "ALTO_IMPACTO" }, { _id: 0, cnpj: 1, competencia: 1, toneladasCO2e: 1, classificacao: 1 }).toArray());

sep("U5 emissoes_carbono: upsert do inventário de uma nova competência");
printjson(db.emissoes_carbono.updateOne(
  { cnpj: "90.123.456/0001-78", competencia: "2026-03", escopo: 2 },
  { $set: { toneladasCO2e: 3.4, fonte: "energia_eletrica", detalhe: { consumoKWh: 44000, origem: "geração própria" } } },
  { upsert: true }
));

sep("U6 licencas_ambientais: baixar condicionante específica do array ($ posicional)");
printjson(db.licencas_ambientais.updateOne(
  { numero: "LO-2023-0451", "condicionantes.descricao": "Relatório de emissões atmosféricas" },
  { $set: { "condicionantes.$.atendida": true, "condicionantes.$.dataAtendimento": ISODate("2026-08-15") } }
));
printjson(db.licencas_ambientais.findOne({ numero: "LO-2023-0451" }, { _id: 0, numero: 1, condicionantes: 1 }));

sep("U7 licencas_ambientais: protocolar renovação ($push em array) e mudar situação");
printjson(db.licencas_ambientais.updateOne(
  { numero: "LO-2020-3345" },
  { $push: { renovacoes: { protocolo: "REN-2026-901", data: ISODate("2026-08-25"), status: "Protocolada" } },
    $set: { situacao: "Em renovação" } }
));
printjson(db.licencas_ambientais.findOne({ numero: "LO-2020-3345" }, { _id: 0, numero: 1, situacao: 1, renovacoes: 1 }));

sep("U8 auditorias_conformidade: registrar plano de ação e reauditoria (findOneAndUpdate)");
printjson(db.auditorias_conformidade.findOneAndUpdate(
  { cnpj: "23.456.789/0001-01", norma: "ISO 14001:2015" },
  { $set: { "naoConformidades.0.status": "Em tratamento", reauditoria: ISODate("2026-09-15") } },
  { returnDocument: "after", projection: { _id: 0, cnpj: 1, naoConformidades: 1, reauditoria: 1 } }
));

sep("U9 alertas_ambientais: resolver em lote alertas de licença já renovada (updateMany)");
printjson(db.alertas_ambientais.updateMany(
  { tipo: "LICENCA_A_VENCER", "dados.numero": "LO-2020-3345", resolvido: false },
  { $set: { resolvido: true, resolvidoEm: ISODate("2026-08-25T10:00:00Z"), "dados.protocoloRenovacao": "REN-2026-901" } }
));
printjson(db.alertas_ambientais.find({ "dados.numero": "LO-2020-3345" }, { _id: 0, tipo: 1, resolvido: 1, dados: 1 }).toArray());

sep("U10 alertas_ambientais: escalar severidade dos alertas em lote ($set + aggregate de conferência)");
printjson(db.alertas_ambientais.updateMany(
  { severidade: "ALTA", resolvido: false },
  { $set: { severidade: "CRITICA", escaladoEm: ISODate("2026-08-26T09:00:00Z"), escaladoPara: "Comitê ESG" } }
));
printjson(db.alertas_ambientais.aggregate([
  { $group: { _id: { severidade: "$severidade", resolvido: "$resolvido" }, total: { $sum: 1 } } }, { $sort: { total: -1 } }
]).toArray());
