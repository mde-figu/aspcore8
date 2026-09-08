// EcoTracker NoSQL — evidência do validador de esquema e exportação dos dados em JSON
db = db.getSiblingDB("ecotracker");
const sep = t => print("\n=============== " + t + " ===============");

sep("V1 tentativa de inserir documento inválido (escopo 4 não existe no GHG Protocol)");
try {
  db.emissoes_carbono.insertOne({ cnpj: "12.345.678/0001-90", escopo: 4, competencia: "2026-04", toneladasCO2e: 10 });
} catch (e) {
  print("REJEITADO PELO VALIDADOR: " + e.errmsg);
  printjson(e.errInfo.details.schemaRulesNotSatisfied);
}

sep("V2 tentativa de violar o índice único de CNPJ");
try {
  db.organizacoes.insertOne({ cnpj: "12.345.678/0001-90", razaoSocial: "Duplicada Ltda.", setor: "Indústria" });
} catch (e) {
  print("REJEITADO PELO ÍNDICE ÚNICO: " + e.errmsg);
}

sep("V3 documento flexível aceito na mesma collection (campos novos, sem migração de schema)");
printjson(db.emissoes_carbono.insertOne({
  cnpj: "12.345.678/0001-90", escopo: 3, competencia: "2026-04", toneladasCO2e: 7.9,
  fonte: "home_office", detalhe: { colaboradores: 88, diasRemotos: 12, metodologia: "GHG Protocol Scope 3 cat. 7" },
  tags: ["piloto", "novo indicador"]
}));
printjson(db.emissoes_carbono.findOne({ fonte: "home_office" }, { _id: 0 }));

sep("V4 exportação das collections para arquivos JSON (EJSON)");
["organizacoes", "emissoes_carbono", "licencas_ambientais", "auditorias_conformidade", "alertas_ambientais"]
  .forEach(c => {
    const docs = db[c].find().toArray();
    fs.writeFileSync(`dados/${c}.json`, EJSON.stringify(docs, null, 2));
    print(`dados/${c}.json — ${docs.length} documentos`);
  });
