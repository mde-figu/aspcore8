# EcoTracker — versão NoSQL (MongoDB)

Migração do modelo de dados do EcoTracker (SQL Server + EF Core) para o modelo documental do
MongoDB. Entrega da atividade "Um novo paradigma com NOT ONLY SQL" (Fase 5 — Data Universe).

## Collections

| Collection | Dimensão ESG | Conteúdo |
| --- | --- | --- |
| `organizacoes` | Governança / Social | empresas monitoradas, score de conformidade, unidades, indicadores de diversidade |
| `emissoes_carbono` | Ambiental | inventário de GEE por escopo (1, 2, 3), fonte, competência e compensações |
| `licencas_ambientais` | Governança | licenças, prazos, condicionantes e renovações |
| `auditorias_conformidade` | Governança / Social | auditorias por norma, checklist com dimensão ESG, não conformidades |
| `alertas_ambientais` | Ambiental / Governança | alertas de limite de emissão, licença a vencer, qualidade do ar e consumo de energia |

## Execução

```bash
mongod --dbpath ./mongodata --bind_ip 127.0.0.1 --port 27017

cd mongodb
mongosh --quiet --file 01_create_collections.js   # collections, validadores $jsonSchema e índices
mongosh --quiet --file 02_insert_documents.js     # CREATE (11 a 16 documentos por collection)
mongosh --quiet --file 03_read_queries.js         # READ   (12 consultas e agregações)
mongosh --quiet --file 04_update_documents.js     # UPDATE (10 operações)
mongosh --quiet --file 05_delete_documents.js     # DELETE (10 operações)
mongosh --quiet --file 06_validacao_e_export.js   # validação de esquema + export JSON em dados/
```

Testado com MongoDB Community 8.0.30 e mongosh 2.10.0. O script 01 executa `dropDatabase()` para
que a evidência seja reproduzível.

`dados/*.json` é o dump (EJSON) das collections após a execução completa dos scripts.
A documentação da atividade está em [`../docs/EcoTracker_NoSQL_MongoDB.pdf`](../docs/EcoTracker_NoSQL_MongoDB.pdf).
