using EcoTracker.BddTests.Support;
using Reqnroll;

namespace EcoTracker.BddTests.Steps;

/// <summary>
/// Pré-condições de negócio (ESG): cria registros via API com um usuário Admin
/// e guarda o ID criado para os próximos passos. Cada cenário cria seus próprios dados,
/// o que permite rodar os testes tanto em memória quanto contra o SQL Server em Docker.
/// </summary>
[Binding]
public sealed class DadosEsgSteps
{
    private readonly ApiContext _api;

    public DadosEsgSteps(ApiContext api) => _api = api;

    [Given("que existe uma emissão de carbono cadastrada para a empresa {string}")]
    public Task EmissaoCadastrada(string empresa) => CriarComoAdmin("/api/CarbonEmissions", "emissaoId",
        $$"""
        {"companyName":"{{empresa}}","sector":"Energia","emissionTonsCO2":800.0,"offsetTonsCO2":200.0,
         "referenceYear":{ano},"referenceMonth":1,"description":"Inventário GEE - escopo 1"}
        """);

    [Given("que existe uma licença ambiental {string} da empresa {string} que vence em {int} dias")]
    public Task LicencaCadastrada(string numero, string empresa, int dias) => CriarComoAdmin("/api/EnvironmentalLicenses", "licencaId",
        $$"""
        {"companyName":"{{empresa}}","licenseNumber":"{{numero}}","licenseType":"Licença de Operação",
         "issuingAuthority":"IBAMA","issueDate":"{hoje-365}","expirationDate":"{hoje+{{dias}}}"}
        """.Replace("{hoje+-", "{hoje-"));

    [Given("que existe uma auditoria de compliance da empresa {string} na norma {string} com score {int}")]
    public Task AuditoriaCadastrada(string empresa, string norma, int score) => CriarComoAdmin("/api/ComplianceAudits", "auditoriaId",
        $$"""
        {"companyName":"{{empresa}}","auditorName":"Auditor BDD","normReference":"{{norma}}",
         "auditDate":"{hoje-7}","complianceScore":{{score}},"findings":"Sem não conformidades maiores"}
        """);

    [Given("que existe um alerta ambiental de severidade {string} em aberto para a empresa {string}")]
    public Task AlertaCadastrado(string severidade, string empresa) => CriarComoAdmin("/api/EnvironmentalAlerts", "alertaId",
        $$"""
        {"companyName":"{{empresa}}","alertType":"EmissionThreshold","severity":"{{severidade}}",
         "message":"Emissão líquida acima do limite do setor"}
        """);

    private async Task CriarComoAdmin(string caminho, string variavel, string corpo)
    {
        var tokenDoCenario = _api.Token;
        _api.Token = await AuthSteps.LoginAsync(_api, "admin");
        await _api.SendAsync("POST", caminho, corpo);
        _api.Token = tokenDoCenario;

        Assert.True((int)_api.Response!.StatusCode == 201, $"Falha ao preparar dados em {caminho}: {(int)_api.Response.StatusCode} {_api.ResponseBody}");
        _api.Variables[variavel] = _api.Select("id")!.ToJsonString();
    }
}
