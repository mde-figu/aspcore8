// EcoTracker NoSQL — DELETE: remoções unitárias, em lote, em arrays e de campos
db = db.getSiblingDB("ecotracker");
const sep = t => print("\n=============== " + t + " ===============");

sep("D0 contagem antes das remoções");
["organizacoes", "emissoes_carbono", "licencas_ambientais", "auditorias_conformidade", "alertas_ambientais"]
  .forEach(c => print(c + ": " + db[c].countDocuments()));

sep("D1 organizacoes: remover cadastro duplicado/encerrado (deleteOne)");
printjson(db.organizacoes.deleteOne({ cnpj: "78.901.234/0001-56" }));
print("organizacoes restantes: " + db.organizacoes.countDocuments());

sep("D2 emissoes_carbono: remover lançamento incorreto (findOneAndDelete devolve o documento)");
printjson(db.emissoes_carbono.findOneAndDelete(
  { cnpj: "01.234.567/0001-89", competencia: "2026-01" },
  { projection: { _id: 0, cnpj: 1, fonte: 1, toneladasCO2e: 1 } }
));

sep("D3 emissoes_carbono: expurgo em lote de lançamentos irrelevantes (< 15 tCO2e)");
printjson(db.emissoes_carbono.deleteMany({ toneladasCO2e: { $lt: 15 } }));
print("emissoes_carbono restantes: " + db.emissoes_carbono.countDocuments());

sep("D4 licencas_ambientais: remover condicionante cancelada de dentro do array ($pull)");
printjson(db.licencas_ambientais.updateOne(
  { numero: "LO-2020-3345" },
  { $pull: { condicionantes: { descricao: "Auditoria externa anual" } } }
));
printjson(db.licencas_ambientais.findOne({ numero: "LO-2020-3345" }, { _id: 0, numero: 1, condicionantes: 1 }));

sep("D5 licencas_ambientais: remover licença arquivada (deleteOne)");
printjson(db.licencas_ambientais.deleteOne({ numero: "LP-2026-0007" }));
print("licencas_ambientais restantes: " + db.licencas_ambientais.countDocuments());

sep("D6 auditorias_conformidade: remover campo obsoleto do documento ($unset)");
printjson(db.auditorias_conformidade.updateMany({ verificacaoRemota: { $exists: true } }, { $unset: { verificacaoRemota: "" } }));

sep("D7 auditorias_conformidade: expurgo de auditorias anteriores a 01/02/2026 (deleteMany)");
printjson(db.auditorias_conformidade.deleteMany({ dataAuditoria: { $lt: ISODate("2026-02-01") } }));
print("auditorias_conformidade restantes: " + db.auditorias_conformidade.countDocuments());

sep("D8 alertas_ambientais: limpar alertas resolvidos (deleteMany)");
printjson(db.alertas_ambientais.deleteMany({ resolvido: true }));
print("alertas_ambientais restantes: " + db.alertas_ambientais.countDocuments());

sep("D9 contagem final de cada collection");
["organizacoes", "emissoes_carbono", "licencas_ambientais", "auditorias_conformidade", "alertas_ambientais"]
  .forEach(c => print(c + ": " + db[c].countDocuments()));
