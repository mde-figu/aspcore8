using System.Text.Json;
using System.Text.Json.Nodes;
using EcoTracker.BddTests.Support;
using Reqnroll;

namespace EcoTracker.BddTests.Steps;

/// <summary>
/// Passos genéricos de API: envio de requisições, validação de status code, corpo JSON e contrato (JSON Schema).
/// </summary>
[Binding]
public sealed class HttpSteps
{
    private readonly ApiContext _api;

    public HttpSteps(ApiContext api) => _api = api;

    [When("eu envio uma requisição {word} para {string}")]
    public Task QuandoEnvioRequisicao(string metodo, string caminho) => _api.SendAsync(metodo, caminho);

    [When("eu envio uma requisição {word} para {string} com o corpo:")]
    public Task QuandoEnvioRequisicaoComCorpo(string metodo, string caminho, string corpo) => _api.SendAsync(metodo, caminho, corpo);

    [Then("o status code da resposta deve ser {int}")]
    public void EntaoStatusCode(int esperado) =>
        Assert.True((int)_api.Response!.StatusCode == esperado,
            $"Status esperado {esperado}, recebido {(int)_api.Response.StatusCode}. Corpo: {_api.ResponseBody}");

    [Then("a resposta deve respeitar o contrato {string}")]
    public void EntaoRespeitaContrato(string schema)
    {
        Assert.False(string.IsNullOrWhiteSpace(_api.ResponseBody), "Resposta sem corpo JSON para validar o contrato.");
        var erros = SchemaValidator.Validate(schema, _api.ResponseBody);
        Assert.True(erros.Count == 0, $"Contrato '{schema}' violado:\n{string.Join("\n", erros)}\nCorpo: {_api.ResponseBody}");
    }

    [Then("o Content-Type da resposta deve ser {string}")]
    public void EntaoContentType(string esperado) =>
        Assert.Equal(esperado, _api.Response!.Content.Headers.ContentType?.MediaType);

    [Then("o corpo da resposta deve ser {string}")]
    public void EntaoCorpoDeveSer(string esperado) => Assert.Equal(esperado, _api.ResponseBody);

    [Then("o corpo da resposta deve estar vazio")]
    public void EntaoCorpoVazio() => Assert.True(string.IsNullOrEmpty(_api.ResponseBody), $"Corpo inesperado: {_api.ResponseBody}");

    [Then("o campo {string} da resposta deve ser {string}")]
    public void EntaoCampoDeveSer(string campo, string esperado) =>
        AssertValue(esperado, _api.Select(campo), campo);

    [Then("o campo {string} da resposta deve conter {string}")]
    public void EntaoCampoDeveConter(string campo, string trecho) =>
        Assert.Contains(_api.Resolve(trecho), Text(_api.Select(campo)));

    [Then("o campo {string} da resposta não deve ser nulo")]
    public void EntaoCampoNaoNulo(string campo) => Assert.NotNull(_api.Select(campo));

    [Then("o campo {string} da resposta deve ser nulo")]
    public void EntaoCampoNulo(string campo) => Assert.Null(_api.Select(campo));

    [Then("o campo {string} da resposta deve ser maior que {int}")]
    public void EntaoCampoMaiorQue(string campo, int limite) =>
        Assert.True(_api.Select(campo)!.GetValue<decimal>() > limite, $"'{campo}' deveria ser > {limite}. Corpo: {_api.ResponseBody}");

    [Then("a resposta deve conter o cabeçalho {string}")]
    public void EntaoCabecalho(string cabecalho) =>
        Assert.True(_api.Response!.Headers.Contains(cabecalho), $"Cabeçalho '{cabecalho}' ausente.");

    [Then("a lista {string} da resposta deve conter ao menos {int} item/itens")]
    public void EntaoListaAoMenos(string caminho, int minimo) =>
        Assert.True(Lista(caminho).Count >= minimo, $"'{caminho}' tem {Lista(caminho).Count} itens, esperado >= {minimo}.");

    [Then("a lista {string} da resposta deve conter no máximo {int} item/itens")]
    public void EntaoListaNoMaximo(string caminho, int maximo) =>
        Assert.True(Lista(caminho).Count <= maximo, $"'{caminho}' tem {Lista(caminho).Count} itens, esperado <= {maximo}.");

    [Then("todos os itens de {string} devem ter o campo {string} igual a {string}")]
    public void EntaoTodosItens(string caminho, string campo, string esperado)
    {
        var itens = Lista(caminho);
        Assert.NotEmpty(itens);
        foreach (var item in itens) AssertValue(esperado, item![campo], campo);
    }

    [Then("algum item de {string} deve ter o campo {string} igual a {string}")]
    public void EntaoAlgumItem(string caminho, string campo, string esperado) =>
        Assert.True(Lista(caminho).Any(i => Matches(_api.Resolve(esperado), i![campo])),
            $"Nenhum item de '{caminho}' com {campo} = {esperado}. Corpo: {_api.ResponseBody}");

    [Then("nenhum item de {string} deve ter o campo {string} igual a {string}")]
    public void EntaoNenhumItem(string caminho, string campo, string esperado) =>
        Assert.False(Lista(caminho).Any(i => Matches(_api.Resolve(esperado), i![campo])),
            $"Item inesperado em '{caminho}' com {campo} = {esperado}. Corpo: {_api.ResponseBody}");

    [Then("a resposta deve conter erro de validação para o campo {string}")]
    public void EntaoErroDeValidacao(string campo)
    {
        var erros = _api.Select("errors")!.AsObject();
        Assert.True(erros.Any(e => e.Key.EndsWith(campo, StringComparison.OrdinalIgnoreCase)),
            $"Erro de validação para '{campo}' não encontrado. Erros: {erros.ToJsonString()}");
    }

    [Then("o tempo de resposta deve ser inferior a {int} ms")]
    public void EntaoTempoDeResposta(int limiteMs) =>
        Assert.True(_api.Elapsed.TotalMilliseconds < limiteMs, $"Resposta levou {_api.Elapsed.TotalMilliseconds:F0} ms (limite {limiteMs} ms).");

    [Then("guardo o campo {string} da resposta como {string}")]
    [Given("guardo o campo {string} da resposta como {string}")]
    public void GuardoCampo(string campo, string variavel) => _api.Variables[variavel] = Text(_api.Select(campo));

    private JsonArray Lista(string caminho) => _api.Select(caminho)?.AsArray() ?? throw new InvalidOperationException($"'{caminho}' não é uma lista.");

    private void AssertValue(string esperado, JsonNode? atual, string campo) =>
        Assert.True(Matches(_api.Resolve(esperado), atual), $"Campo '{campo}': esperado '{_api.Resolve(esperado)}', recebido '{Text(atual)}'. Corpo: {_api.ResponseBody}");

    private static bool Matches(string esperado, JsonNode? atual)
    {
        if (atual is null) return esperado == "null";
        if (atual is JsonValue v && v.GetValueKind() == JsonValueKind.Number &&
            decimal.TryParse(esperado, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var numero))
            return v.GetValue<decimal>() == numero;
        return string.Equals(Text(atual), esperado, StringComparison.Ordinal);
    }

    private static string Text(JsonNode? node) => node switch
    {
        null => "null",
        JsonValue v when v.GetValueKind() == JsonValueKind.String => v.GetValue<string>(),
        _ => node.ToJsonString()
    };
}
