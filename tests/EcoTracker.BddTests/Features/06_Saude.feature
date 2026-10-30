# language: pt
@infra @eficiencia
Funcionalidade: Disponibilidade e eficiência da API
  Como operador da plataforma
  Quero um endpoint leve de health check
  Para monitorar a disponibilidade sem consumo desnecessário de recursos computacionais

  @positivo
  Cenário: Health check responde rapidamente
    Quando eu envio uma requisição GET para "/health"
    Então o status code da resposta deve ser 200
    E o corpo da resposta deve ser "Healthy"
    E o tempo de resposta deve ser inferior a 1000 ms

  @negativo
  Cenário: Rota inexistente retorna 404
    Quando eu envio uma requisição GET para "/api/RecursoInexistente"
    Então o status code da resposta deve ser 404
