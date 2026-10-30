# language: pt
@governanca @auditorias
Funcionalidade: Auditorias de compliance ambiental
  Como auditor ambiental
  Quero registrar auditorias de conformidade (ISO 14001, ISO 14064, ...)
  Para manter a rastreabilidade das inspeções e o score de conformidade das empresas

  @positivo @contrato
  Cenário: Registrar auditoria inicia com resultado pendente
    Dado que estou autenticado como "auditor"
    Quando eu envio uma requisição POST para "/api/ComplianceAudits" com o corpo:
      """
      {
        "companyName": "Papel e Celulose {unico}",
        "auditorName": "Ana Pereira",
        "normReference": "ISO 14001:2015",
        "auditDate": "{hoje-2}",
        "complianceScore": 88,
        "findings": "Plano de gestão de resíduos implementado",
        "nextAuditDate": "{hoje+365}"
      }
      """
    Então o status code da resposta deve ser 201
    E a resposta deve respeitar o contrato "compliance-audit.schema.json"
    E o campo "result" da resposta deve ser "Pending"
    E o campo "complianceScore" da resposta deve ser "88"
    E o campo "nextAuditDate" da resposta não deve ser nulo

  @positivo @contrato
  Cenário: Consultar auditorias filtrando pela norma
    Dado que existe uma auditoria de compliance da empresa "Cimenteira {unico}" na norma "ISO 14064-{unico}" com score 70
    Quando eu envio uma requisição GET para "/api/ComplianceAudits?norm=ISO 14064-{unico}"
    Então o status code da resposta deve ser 200
    E a resposta deve respeitar o contrato "compliance-audit-page.schema.json"
    E o campo "totalCount" da resposta deve ser "1"
    E o campo "items[0].companyName" da resposta deve ser "Cimenteira {unico}"
    Quando eu envio uma requisição GET para "/api/ComplianceAudits/{auditoriaId}"
    Então o status code da resposta deve ser 200
    E a resposta deve respeitar o contrato "compliance-audit.schema.json"

  @positivo
  Cenário: Concluir auditoria com aprovação condicional e ações corretivas
    Dado que existe uma auditoria de compliance da empresa "Textil {unico}" na norma "ISO 14001:2015" com score 65
    E que estou autenticado como "auditor"
    Quando eu envio uma requisição PUT para "/api/ComplianceAudits/{auditoriaId}" com o corpo:
      """
      { "result": "ConditionalApproval", "complianceScore": 72, "correctiveActions": "Tratar efluentes antes do descarte" }
      """
    Então o status code da resposta deve ser 200
    E a resposta deve respeitar o contrato "compliance-audit.schema.json"
    E o campo "result" da resposta deve ser "ConditionalApproval"
    E o campo "correctiveActions" da resposta deve conter "efluentes"

  @positivo
  Cenário: Administrador remove auditoria
    Dado que existe uma auditoria de compliance da empresa "Removivel {unico}" na norma "ISO 50001" com score 90
    E que estou autenticado como "admin"
    Quando eu envio uma requisição DELETE para "/api/ComplianceAudits/{auditoriaId}"
    Então o status code da resposta deve ser 204

  @negativo @contrato
  Esquema do Cenário: Score de conformidade fora da faixa 0-100 é rejeitado
    Dado que estou autenticado como "auditor"
    Quando eu envio uma requisição POST para "/api/ComplianceAudits" com o corpo:
      """
      { "companyName": "Score Invalido", "auditorName": "X", "normReference": "ISO 14001", "auditDate": "{hoje}", "complianceScore": <score> }
      """
    Então o status code da resposta deve ser 400
    E a resposta deve respeitar o contrato "validation-problem.schema.json"
    E a resposta deve conter erro de validação para o campo "ComplianceScore"

    Exemplos:
      | score |
      | -1    |
      | 101   |

  @negativo @governanca
  Cenário: Perfil Viewer não pode excluir auditorias (trilha de auditoria preservada)
    Dado que existe uma auditoria de compliance da empresa "Imutavel {unico}" na norma "ISO 14001:2015" com score 95
    E que estou autenticado como "viewer"
    Quando eu envio uma requisição DELETE para "/api/ComplianceAudits/{auditoriaId}"
    Então o status code da resposta deve ser 403
    Quando eu envio uma requisição GET para "/api/ComplianceAudits/{auditoriaId}"
    Então o status code da resposta deve ser 200

  @negativo @contrato
  Cenário: Auditoria inexistente retorna 404
    Quando eu envio uma requisição GET para "/api/ComplianceAudits/999999"
    Então o status code da resposta deve ser 404
    E a resposta deve respeitar o contrato "error-message.schema.json"
