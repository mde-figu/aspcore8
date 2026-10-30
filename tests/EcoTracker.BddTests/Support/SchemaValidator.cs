using System.Text.Json.Nodes;
using Json.Schema;

namespace EcoTracker.BddTests.Support;

/// <summary>
/// Testes de contrato: valida o JSON da resposta contra os JSON Schemas (draft 2020-12) da pasta Schemas/.
/// Todos os schemas são registrados para que $ref entre arquivos funcione.
/// </summary>
public static class SchemaValidator
{
    private static readonly Lazy<Dictionary<string, JsonSchema>> Schemas = new(Load);

    private static Dictionary<string, JsonSchema> Load()
    {
        var dir = Path.Combine(AppContext.BaseDirectory, "Schemas");
        var schemas = new Dictionary<string, JsonSchema>(StringComparer.OrdinalIgnoreCase);

        foreach (var file in Directory.GetFiles(dir, "*.schema.json"))
        {
            var schema = JsonSchema.FromText(File.ReadAllText(file));
            SchemaRegistry.Global.Register(schema);
            schemas[Path.GetFileName(file)] = schema;
        }

        return schemas;
    }

    public static IReadOnlyList<string> Validate(string schemaFile, string json)
    {
        if (!Schemas.Value.TryGetValue(schemaFile, out var schema))
            throw new FileNotFoundException($"Schema '{schemaFile}' não encontrado em Schemas/.");

        var result = schema.Evaluate(JsonNode.Parse(json), new EvaluationOptions { OutputFormat = OutputFormat.List });

        return result.IsValid
            ? Array.Empty<string>()
            : result.Details
                .Where(d => d.HasErrors)
                .SelectMany(d => d.Errors!.Select(e => $"{d.InstanceLocation}: {e.Value}"))
                .ToList();
    }
}
