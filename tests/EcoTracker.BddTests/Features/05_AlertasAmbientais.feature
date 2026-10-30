# language: pt
@ambiental @alertas
Funcionalidade: Alertas ambientais
  Como gestor ambiental
  Quero registrar e resolver alertas (limite de emissão, licença vencendo, vazamentos)
  Para agir rapidamente sobre riscos ambientais e documentar a resolução

  @positivo @contrato
  Cenário: Registrar um alerta crítico de emissão
    Dado que estou autenticado como "auditor"
    Quando eu envio uma requisição POST para "/api/EnvironmentalAlerts" com o corpo:
      """
      {
        "companyName": "Refinaria {unico}",
        "alertType": "EmissionThreshold",
        "severity": "Critical",
        "message": "Emissão de SO2 acima do limite CONAMA",
        "relatedLicenseNumber": "LO-REF-{unico}"
      }
      """
    Então o status code da resposta deve ser 201
    E a resposta deve respeitar o contrato "environmental-alert.schema.json"
    E o campo "isResolved" da resposta deve ser "false"
    E o campo "resolvedAt" da resposta deve ser nulo

  @positivo @contrato
  Cenário: Listar alertas em aberto por severidade
    Dado que existe um alerta ambiental de severidade "High" em aberto para a empresa "Porto {unico}"
    Quando eu envio uma requisição GET para "/api/EnvironmentalAlerts?company=Porto {unico}&severity=High&resolved=false"
    Então o status code da resposta deve ser 200
    E a resposta deve respeitar o contrato "environmental-alert-page.schema.json"
    E o campo "totalCount" da resposta deve ser "1"
    E todos os itens de "items" devem ter o campo "isResolved" igual a "false"
    Quando eu envio uma requisição GET para "/api/EnvironmentalAlerts/{alertaId}"
    Então o status code da resposta deve ser 200
    E a resposta deve respeitar o contrato "environmental-alert.schema.json"
    E o campo "severity" da resposta deve ser "High"

  @positivo @contrato
  Cenário: Resolver um alerta registra data e notas de resolução
    Dado que existe um alerta ambiental de severidade "Medium" em aberto para a empresa "Laticinio {unico}"
    E que estou autenticado como "auditor"
    Quando eu envio uma requisição PATCH para "/api/EnvironmentalAlerts/{alertaId}/resolve" com o corpo:
      """
      { "resolutionNotes": "Filtro de efluente substituído e laudo emitido" }
      """
    Então o status code da resposta deve ser 200
    E a resposta deve respeitar o contrato "environmental-alert.schema.json"
    E o campo "isResolved" da resposta deve ser "true"
    E o campo "resolvedAt" da resposta não deve ser nulo
    E o campo "resolutionNotes" da resposta deve ser "Filtro de efluente substituído e laudo emitido"
    Quando eu envio uma requisição GET para "/api/EnvironmentalAlerts?company=Laticinio {unico}&resolved=false"
    Então o campo "totalCount" da resposta deve ser "0"

  @positivo
  Cenário: Administrador exclui um alerta
    Dado que existe um alerta ambiental de severidade "Low" em aberto para a empresa "Falso Positivo {unico}"
    E que estou autenticado como "admin"
    Quando eu envio uma requisição DELETE para "/api/EnvironmentalAlerts/{alertaId}"
    Então o status code da resposta deve ser 204

  @negativo @contrato
  Cenário: Severidade fora do padrão é rejeitada
    Dado que estou autenticado como "auditor"
    Quando eu envio uma requisição POST para "/api/EnvironmentalAlerts" com o corpo:
      """
      { "companyName": "X", "alertType": "Leak", "severity": "Urgentissimo", "message": "Vazamento" }
      """
    Então o status code da resposta deve ser 400
    E a resposta deve respeitar o contrato "validation-problem.schema.json"
    E a resposta deve conter erro de validação para o campo "Severity"

  @negativo @contrato
  Cenário: Resolver alerta sem notas de resolução é rejeitado
    Dado que existe um alerta ambiental de severidade "High" em aberto para a empresa "Sem Notas {unico}"
    E que estou autenticado como "auditor"
    Quando eu envio uma requisição PATCH para "/api/EnvironmentalAlerts/{alertaId}/resolve" com o corpo:
      """
      { }
      """
    Então o status code da resposta deve ser 400
    E a resposta deve respeitar o contrato "validation-problem.schema.json"
    E a resposta deve conter erro de validação para o campo "ResolutionNotes"

  @negativo @contrato
  Cenário: Resolver alerta inexistente retorna 404
    Dado que estou autenticado como "auditor"
    Quando eu envio uma requisição PATCH para "/api/EnvironmentalAlerts/999999/resolve" com o corpo:
      """
      { "resolutionNotes": "N/A" }
      """
    Então o status code da resposta deve ser 404
    E a resposta deve respeitar o contrato "error-message.schema.json"

  @negativo
  Cenário: Resolver alerta sem autenticação é bloqueado
    Dado que existe um alerta ambiental de severidade "High" em aberto para a empresa "Anonimo {unico}"
    E que não estou autenticado
    Quando eu envio uma requisição PATCH para "/api/EnvironmentalAlerts/{alertaId}/resolve" com o corpo:
      """
      { "resolutionNotes": "Tentativa sem login" }
      """
    Então o status code da resposta deve ser 401
