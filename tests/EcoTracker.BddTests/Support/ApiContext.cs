using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace EcoTracker.BddTests.Support;

/// <summary>
/// Estado de um cenário: cliente HTTP, token JWT, última resposta e variáveis guardadas entre passos.
/// Uma instância por cenário (injeção de contexto do Reqnroll).
/// </summary>
public sealed class ApiContext : IDisposable
{
    private static readonly Regex Placeholder = new(@"\{([A-Za-z0-9_]+)([+-]\d+)?\}", RegexOptions.Compiled);

    public ApiContext()
    {
        Client = ApiHost.CreateClient();
        UniqueSuffix = Guid.NewGuid().ToString("N")[..8];
    }

    public HttpClient Client { get; }
    public string UniqueSuffix { get; }
    public string? Token { get; set; }
    public HttpResponseMessage? Response { get; private set; }
    public string ResponseBody { get; private set; } = string.Empty;
    public Dictionary<string, string> Variables { get; } = new(StringComparer.OrdinalIgnoreCase);

    public TimeSpan Elapsed { get; private set; }

    public JsonNode? Json => string.IsNullOrWhiteSpace(ResponseBody) ? null : JsonNode.Parse(ResponseBody);

    public async Task SendAsync(string method, string path, string? body = null)
    {
        var request = new HttpRequestMessage(new HttpMethod(method), Resolve(path).TrimStart('/'));

        if (!string.IsNullOrEmpty(Token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);

        if (body is not null)
            request.Content = new StringContent(Resolve(body), Encoding.UTF8, "application/json");

        Response?.Dispose();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        Response = await Client.SendAsync(request);
        Elapsed = stopwatch.Elapsed;
        ResponseBody = await Response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Substitui marcadores no texto:
    /// {unico} → sufixo único do cenário; {hoje+30} / {hoje-10} → data ISO relativa a hoje;
    /// {ano} → ano corrente; {qualquerVariavel} → valor guardado com "guardo o campo ... como ...".
    /// </summary>
    public string Resolve(string text) => Placeholder.Replace(text, m =>
    {
        var name = m.Groups[1].Value;
        var offset = m.Groups[2].Success ? int.Parse(m.Groups[2].Value) : 0;

        return name.ToLowerInvariant() switch
        {
            "unico" => UniqueSuffix,
            "hoje" => DateTime.UtcNow.Date.AddDays(offset).ToString("yyyy-MM-ddTHH:mm:ss"),
            "ano" => (DateTime.UtcNow.Year + offset).ToString(),
            _ when Variables.TryGetValue(name, out var value) => value,
            _ => m.Value
        };
    });

    public JsonNode? Select(string path)
    {
        JsonNode? node = Json;
        if (path == "$") return node;
        foreach (var segment in Regex.Matches(path, @"[^.\[\]]+|\[\d+\]").Select(m => m.Value))
        {
            if (node is null) return null;
            node = segment.StartsWith('[')
                ? node.AsArray().ElementAtOrDefault(int.Parse(segment.Trim('[', ']')))
                : node.AsObject().TryGetPropertyValue(segment, out var child) ? child : throw new KeyNotFoundException(
                    $"Campo '{segment}' não existe na resposta. Corpo: {ResponseBody}");
        }
        return node;
    }

    public void Dispose()
    {
        Response?.Dispose();
        Client.Dispose();
    }
}
