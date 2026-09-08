// EcoTracker NoSQL — criação do banco e das 5 collections (MongoDB 8.0)
// Execução: mongosh --file 01_create_collections.js

db = db.getSiblingDB("ecotracker");

// Recria o ambiente do zero para a evidência ser reproduzível
db.dropDatabase();

// 1) organizacoes — GOVERNANÇA + SOCIAL: cadastro das empresas/unidades monitoradas
db.createCollection("organizacoes", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["cnpj", "razaoSocial", "setor"],
      properties: {
        cnpj: { bsonType: "string", description: "CNPJ da organização" },
        razaoSocial: { bsonType: "string" },
        setor: { bsonType: "string" },
        scoreConformidade: { bsonType: ["double", "int"], minimum: 0, maximum: 100 }
      }
    }
  },
  validationLevel: "moderate" // permite esquema flexível nos demais campos
});
db.organizacoes.createIndex({ cnpj: 1 }, { unique: true });
db.organizacoes.createIndex({ setor: 1, scoreConformidade: -1 });

// 2) emissoes_carbono — AMBIENTAL: inventário de emissões (escopos 1, 2 e 3)
db.createCollection("emissoes_carbono", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["cnpj", "escopo", "toneladasCO2e", "competencia"],
      properties: {
        escopo: { enum: [1, 2, 3] },
        toneladasCO2e: { bsonType: ["double", "int"], minimum: 0 },
        competencia: { bsonType: "string", pattern: "^[0-9]{4}-[0-9]{2}$" }
      }
    }
  },
  validationLevel: "moderate"
});
db.emissoes_carbono.createIndex({ cnpj: 1, competencia: -1 });
db.emissoes_carbono.createIndex({ escopo: 1 });

// 3) licencas_ambientais — GOVERNANÇA: licenças, condicionantes e renovações
db.createCollection("licencas_ambientais", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["cnpj", "numero", "tipo", "validade"],
      properties: {
        tipo: { enum: ["LP", "LI", "LO", "Outorga", "AAF"] },
        validade: { bsonType: "date" },
        condicionantes: { bsonType: "array" }
      }
    }
  },
  validationLevel: "moderate"
});
db.licencas_ambientais.createIndex({ numero: 1 }, { unique: true });
db.licencas_ambientais.createIndex({ validade: 1 });

// 4) auditorias_conformidade — GOVERNANÇA + SOCIAL: auditorias com checklist embutido
db.createCollection("auditorias_conformidade", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["cnpj", "norma", "dataAuditoria"],
      properties: {
        norma: { bsonType: "string" },
        dataAuditoria: { bsonType: "date" },
        itens: { bsonType: "array" }
      }
    }
  },
  validationLevel: "moderate"
});
db.auditorias_conformidade.createIndex({ cnpj: 1, dataAuditoria: -1 });
db.auditorias_conformidade.createIndex({ "itens.dimensao": 1 });

// 5) alertas_ambientais — AMBIENTAL + GOVERNANÇA: alertas automáticos (payload heterogêneo)
db.createCollection("alertas_ambientais", {
  validator: {
    $jsonSchema: {
      bsonType: "object",
      required: ["cnpj", "tipo", "severidade", "criadoEm"],
      properties: {
        tipo: { enum: ["LIMITE_EMISSAO", "LICENCA_A_VENCER", "NAO_CONFORMIDADE", "QUALIDADE_AR", "CONSUMO_ENERGIA"] },
        severidade: { enum: ["BAIXA", "MEDIA", "ALTA", "CRITICA"] },
        criadoEm: { bsonType: "date" }
      }
    }
  },
  validationLevel: "moderate"
});
db.alertas_ambientais.createIndex({ cnpj: 1, criadoEm: -1 });
db.alertas_ambientais.createIndex({ severidade: 1, resolvido: 1 });

print("Banco em uso: " + db.getName());
printjson(db.getCollectionNames());
