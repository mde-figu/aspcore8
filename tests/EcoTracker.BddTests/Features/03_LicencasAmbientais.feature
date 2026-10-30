# language: pt
@compliance @licencas
Funcionalidade: Controle de licenças ambientais
  Como responsável por compliance ambiental
  Quero controlar as licenças ambientais e seus vencimentos
  Para evitar operar com licença vencida e sofrer sanções regulatórias

  @positivo @contrato
  Cenário: Listar licenças ambientais ativas
    Dado que existe uma licença ambiental "LO-{unico}" da empresa "Mineradora {unico}" que vence em 400 dias
    Quando eu envio uma requisição GET para "/api/EnvironmentalLicenses?status=Active&company=Mineradora {unico}"
    Então o status code da resposta deve ser 200
    E a resposta deve respeitar o contrato "environmental-license-page.schema.json"
    E todos os itens de "items" devem ter o campo "status" igual a "Active"
    E algum item de "items" deve ter o campo "licenseNumber" igual a "LO-{unico}"

  @positivo @contrato
  Cenário: Cadastrar uma nova licença ambiental
    Dado que estou autenticado como "admin"
    Quando eu envio uma requisição POST para "/api/EnvironmentalLicenses" com o corpo:
      """
      {
        "companyName": "Aterro Sanitario {unico}",
        "licenseNumber": "LI-{unico}",
        "licenseType": "Licença de Instalação",
        "issuingAuthority": "CETESB",
        "issueDate": "{hoje-30}",
        "expirationDate": "{hoje+730}",
        "notes": "Gestão de resíduos sólidos urbanos"
      }
      """
    Então o status code da resposta deve ser 201
    E a resposta deve respeitar o contrato "environmental-license.schema.json"
    E o campo "status" da resposta deve ser "Active"
    E o campo "daysUntilExpiration" da resposta deve ser maior que 700
    E guardo o campo "id" da resposta como "novaLicenca"
    Quando eu envio uma requisição GET para "/api/EnvironmentalLicenses/{novaLicenca}"
    Então o status code da resposta deve ser 200
    E a resposta deve respeitar o contrato "environmental-license.schema.json"
    E o campo "licenseNumber" da resposta deve ser "LI-{unico}"

  @positivo @contrato
  Cenário: Licenças próximas do vencimento aparecem no monitoramento preventivo
    Dado que existe uma licença ambiental "VENCE-{unico}" da empresa "Quimica {unico}" que vence em 20 dias
    E que existe uma licença ambiental "LONGA-{unico}" da empresa "Quimica {unico}" que vence em 900 dias
    Quando eu envio uma requisição GET para "/api/EnvironmentalLicenses/expiring?days=60"
    Então o status code da resposta deve ser 200
    E a resposta deve respeitar o contrato "environmental-license-list.schema.json"
    E algum item de "$" deve ter o campo "licenseNumber" igual a "VENCE-{unico}"
    E nenhum item de "$" deve ter o campo "licenseNumber" igual a "LONGA-{unico}"

  @positivo
  Cenário: Suspender uma licença ambiental
    Dado que existe uma licença ambiental "SUSP-{unico}" da empresa "Frigorifico {unico}" que vence em 180 dias
    E que estou autenticado como "auditor"
    Quando eu envio uma requisição PUT para "/api/EnvironmentalLicenses/{licencaId}" com o corpo:
      """
      { "status": "Suspended", "notes": "Suspensa por descumprimento de condicionante" }
      """
    Então o status code da resposta deve ser 200
    E a resposta deve respeitar o contrato "environmental-license.schema.json"
    E o campo "status" da resposta deve ser "Suspended"
    Quando eu envio uma requisição GET para "/api/EnvironmentalLicenses/expiring?days=365"
    Então nenhum item de "$" deve ter o campo "licenseNumber" igual a "SUSP-{unico}"

  @positivo
  Cenário: Administrador exclui uma licença
    Dado que existe uma licença ambiental "DEL-{unico}" da empresa "Temporaria {unico}" que vence em 90 dias
    E que estou autenticado como "admin"
    Quando eu envio uma requisição DELETE para "/api/EnvironmentalLicenses/{licencaId}"
    Então o status code da resposta deve ser 204
    Quando eu envio uma requisição GET para "/api/EnvironmentalLicenses/{licencaId}"
    Então o status code da resposta deve ser 404
    E a resposta deve respeitar o contrato "error-message.schema.json"

  @negativo @contrato
  Cenário: Cadastro de licença sem campos obrigatórios é rejeitado
    Dado que estou autenticado como "admin"
    Quando eu envio uma requisição POST para "/api/EnvironmentalLicenses" com o corpo:
      """
      { "companyName": "Sem Numero SA", "licenseType": "Licença Prévia" }
      """
    Então o status code da resposta deve ser 400
    E a resposta deve respeitar o contrato "validation-problem.schema.json"
    E a resposta deve conter erro de validação para o campo "LicenseNumber"
    E a resposta deve conter erro de validação para o campo "IssuingAuthority"

  @negativo
  Cenário: Visitante não autenticado não pode cadastrar licença
    Dado que não estou autenticado
    Quando eu envio uma requisição POST para "/api/EnvironmentalLicenses" com o corpo:
      """
      { "companyName": "X", "licenseNumber": "X-1", "licenseType": "LP", "issuingAuthority": "IBAMA", "issueDate": "{hoje}", "expirationDate": "{hoje+10}" }
      """
    Então o status code da resposta deve ser 401

  @negativo @governanca
  Cenário: Perfil Viewer não pode excluir licenças
    Dado que existe uma licença ambiental "VIEW-{unico}" da empresa "Leitura {unico}" que vence em 90 dias
    E que estou autenticado como "viewer"
    Quando eu envio uma requisição DELETE para "/api/EnvironmentalLicenses/{licencaId}"
    Então o status code da resposta deve ser 403

  @negativo @contrato
  Cenário: Atualizar licença inexistente retorna 404
    Dado que estou autenticado como "admin"
    Quando eu envio uma requisição PUT para "/api/EnvironmentalLicenses/999999" com o corpo:
      """
      { "status": "Expired" }
      """
    Então o status code da resposta deve ser 404
    E a resposta deve respeitar o contrato "error-message.schema.json"
