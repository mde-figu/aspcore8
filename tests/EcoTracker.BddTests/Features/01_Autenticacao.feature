# language: pt
@governanca @auth
Funcionalidade: Autenticação e controle de acesso (Governança)
  Como responsável pela governança ESG da organização
  Quero que apenas usuários autenticados alterem dados ambientais
  Para garantir integridade, rastreabilidade e segregação de funções (Admin, Auditor, Viewer)

  @positivo @contrato
  Esquema do Cenário: Login com credenciais válidas retorna token JWT para cada perfil
    Quando eu envio uma requisição POST para "/api/Auth/login" com o corpo:
      """
      { "username": "<usuario>", "password": "<senha>" }
      """
    Então o status code da resposta deve ser 200
    E a resposta deve respeitar o contrato "auth-token.schema.json"
    E o campo "username" da resposta deve ser "<usuario>"
    E o campo "role" da resposta deve ser "<perfil>"

    Exemplos:
      | usuario | senha       | perfil  |
      | admin   | Admin@123   | Admin   |
      | auditor | Auditor@123 | Auditor |
      | viewer  | Viewer@123  | Viewer  |

  @negativo @contrato
  Cenário: Login com senha incorreta é recusado
    Quando eu envio uma requisição POST para "/api/Auth/login" com o corpo:
      """
      { "username": "admin", "password": "senha-errada" }
      """
    Então o status code da resposta deve ser 401
    E a resposta deve respeitar o contrato "error-message.schema.json"
    E o campo "message" da resposta deve ser "Credenciais inválidas."

  @negativo @contrato
  Cenário: Login sem informar a senha retorna erro de validação
    Quando eu envio uma requisição POST para "/api/Auth/login" com o corpo:
      """
      { "username": "admin" }
      """
    Então o status code da resposta deve ser 400
    E a resposta deve respeitar o contrato "validation-problem.schema.json"
    E a resposta deve conter erro de validação para o campo "Password"

  @negativo
  Cenário: Token JWT adulterado não permite registrar dados ambientais
    Dado que possuo um token JWT inválido
    Quando eu envio uma requisição POST para "/api/CarbonEmissions" com o corpo:
      """
      { "companyName": "Fraude SA", "sector": "Energia", "emissionTonsCO2": 1, "referenceYear": 2026, "referenceMonth": 1 }
      """
    Então o status code da resposta deve ser 401
