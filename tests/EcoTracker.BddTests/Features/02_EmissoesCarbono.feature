# language: pt
@ambiental @emissoes
Funcionalidade: Gestão de emissões de carbono
  Como gestor de sustentabilidade
  Quero registrar emissões e compensações de CO2 da empresa
  Para acompanhar a pegada de carbono líquida e cumprir metas de descarbonização

  @positivo @contrato
  Cenário: Listar emissões de carbono com paginação
    Quando eu envio uma requisição GET para "/api/CarbonEmissions?page=1&pageSize=5"
    Então o status code da resposta deve ser 200
    E a resposta deve respeitar o contrato "carbon-emission-page.schema.json"
    E o campo "page" da resposta deve ser "1"
    E o campo "pageSize" da resposta deve ser "5"
    E a lista "items" da resposta deve conter no máximo 5 itens
    E o tempo de resposta deve ser inferior a 2000 ms

  @positivo @contrato
  Cenário: Registrar uma emissão calcula a emissão líquida (emissão - compensação)
    Dado que estou autenticado como "auditor"
    Quando eu envio uma requisição POST para "/api/CarbonEmissions" com o corpo:
      """
      {
        "companyName": "Usina Solar {unico}",
        "sector": "Energia Renovável",
        "emissionTonsCO2": 1200.5,
        "offsetTonsCO2": 200.5,
        "referenceYear": 2026,
        "referenceMonth": 9,
        "description": "Inventário GEE - escopo 1 e 2"
      }
      """
    Então o status code da resposta deve ser 201
    E a resposta deve respeitar o contrato "carbon-emission.schema.json"
    E a resposta deve conter o cabeçalho "Location"
    E o campo "companyName" da resposta deve ser "Usina Solar {unico}"
    E o campo "netEmission" da resposta deve ser "1000"
    E guardo o campo "id" da resposta como "novaEmissao"
    Quando eu envio uma requisição GET para "/api/CarbonEmissions/{novaEmissao}"
    Então o status code da resposta deve ser 200
    E o campo "id" da resposta deve ser "{novaEmissao}"

  @positivo
  Cenário: Filtrar emissões por empresa e ano
    Dado que existe uma emissão de carbono cadastrada para a empresa "Siderurgica {unico}"
    Quando eu envio uma requisição GET para "/api/CarbonEmissions?company=Siderurgica {unico}&year={ano}"
    Então o status code da resposta deve ser 200
    E a resposta deve respeitar o contrato "carbon-emission-page.schema.json"
    E o campo "totalCount" da resposta deve ser "1"
    E todos os itens de "items" devem ter o campo "companyName" igual a "Siderurgica {unico}"

  @positivo @contrato
  Cenário: Atualizar a compensação de uma emissão existente
    Dado que existe uma emissão de carbono cadastrada para a empresa "Transportadora {unico}"
    E que estou autenticado como "auditor"
    Quando eu envio uma requisição PUT para "/api/CarbonEmissions/{emissaoId}" com o corpo:
      """
      { "offsetTonsCO2": 500, "description": "Reflorestamento de 20 ha" }
      """
    Então o status code da resposta deve ser 200
    E a resposta deve respeitar o contrato "carbon-emission.schema.json"
    E o campo "offsetTonsCO2" da resposta deve ser "500"
    E o campo "netEmission" da resposta deve ser "300"

  @positivo
  Cenário: Administrador remove uma emissão registrada por engano
    Dado que existe uma emissão de carbono cadastrada para a empresa "Duplicada {unico}"
    E que estou autenticado como "admin"
    Quando eu envio uma requisição DELETE para "/api/CarbonEmissions/{emissaoId}"
    Então o status code da resposta deve ser 204
    E o corpo da resposta deve estar vazio
    Quando eu envio uma requisição GET para "/api/CarbonEmissions/{emissaoId}"
    Então o status code da resposta deve ser 404

  @negativo @contrato
  Esquema do Cenário: Registro de emissão com dados inválidos é rejeitado
    Dado que estou autenticado como "admin"
    Quando eu envio uma requisição POST para "/api/CarbonEmissions" com o corpo:
      """
      { "companyName": "<empresa>", "sector": "Energia", "emissionTonsCO2": <emissao>, "referenceYear": <ano>, "referenceMonth": <mes> }
      """
    Então o status code da resposta deve ser 400
    E a resposta deve respeitar o contrato "validation-problem.schema.json"
    E a resposta deve conter erro de validação para o campo "<campo>"

    Exemplos:
      | caso                 | empresa  | emissao | ano  | mes | campo           |
      | emissão negativa     | EcoCorp  | -10     | 2026 | 1   | EmissionTonsCO2 |
      | mês inexistente      | EcoCorp  | 10      | 2026 | 13  | ReferenceMonth  |
      | ano fora do período  | EcoCorp  | 10      | 1999 | 1   | ReferenceYear   |
      | empresa não informada|          | 10      | 2026 | 1   | CompanyName     |

  @negativo
  Cenário: Registrar emissão sem autenticação é bloqueado
    Dado que não estou autenticado
    Quando eu envio uma requisição POST para "/api/CarbonEmissions" com o corpo:
      """
      { "companyName": "Anonima", "sector": "Energia", "emissionTonsCO2": 10, "referenceYear": 2026, "referenceMonth": 1 }
      """
    Então o status code da resposta deve ser 401

  @negativo @governanca
  Cenário: Perfil Auditor não pode excluir emissões (segregação de funções)
    Dado que existe uma emissão de carbono cadastrada para a empresa "Protegida {unico}"
    E que estou autenticado como "auditor"
    Quando eu envio uma requisição DELETE para "/api/CarbonEmissions/{emissaoId}"
    Então o status code da resposta deve ser 403

  @negativo @contrato
  Esquema do Cenário: Operações em emissão inexistente retornam 404
    Dado que estou autenticado como "admin"
    Quando eu envio uma requisição <metodo> para "/api/CarbonEmissions/999999" com o corpo:
      """
      { "offsetTonsCO2": 1 }
      """
    Então o status code da resposta deve ser 404
    E a resposta deve respeitar o contrato "error-message.schema.json"

    Exemplos:
      | metodo |
      | GET    |
      | PUT    |
      | DELETE |
