using EcoTracker.BddTests.Support;
using Reqnroll;

namespace EcoTracker.BddTests.Steps;

[Binding]
public sealed class AuthSteps
{
    public static readonly IReadOnlyDictionary<string, string> DemoPasswords = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["admin"] = "Admin@123",
        ["auditor"] = "Auditor@123",
        ["viewer"] = "Viewer@123"
    };

    private readonly ApiContext _api;

    public AuthSteps(ApiContext api) => _api = api;

    [Given("que estou autenticado como {string}")]
    public async Task DadoQueEstouAutenticadoComo(string usuario)
    {
        _api.Token = await LoginAsync(_api, usuario);
    }

    [Given("que não estou autenticado")]
    public void DadoQueNaoEstouAutenticado() => _api.Token = null;

    [Given("que possuo um token JWT inválido")]
    public void DadoQuePossuoUmTokenJwtInvalido() =>
        _api.Token = "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJoYWNrZXIifQ.assinatura-invalida";

    public static async Task<string> LoginAsync(ApiContext api, string usuario)
    {
        var previousToken = api.Token;
        api.Token = null;
        await api.SendAsync("POST", "/api/Auth/login",
            $$"""{"username":"{{usuario}}","password":"{{DemoPasswords[usuario]}}"}""");
        api.Token = previousToken;

        Assert.True(api.Response!.IsSuccessStatusCode, $"Falha no login de '{usuario}': {(int)api.Response.StatusCode} {api.ResponseBody}");
        return api.Select("token")!.GetValue<string>();
    }
}
