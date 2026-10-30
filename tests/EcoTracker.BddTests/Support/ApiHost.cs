using EcoTracker.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EcoTracker.BddTests.Support;

/// <summary>
/// Define onde a API sob teste está rodando.
/// - Sem API_BASE_URL: sobe a API em memória (WebApplicationFactory + EF InMemory).
/// - Com API_BASE_URL (ex.: http://localhost:8080): testa uma API real (Docker Compose / staging).
/// </summary>
public static class ApiHost
{
    private static readonly object Lock = new();
    private static InMemoryEcoTrackerFactory? _factory;

    public static string? BaseUrl => Environment.GetEnvironmentVariable("API_BASE_URL");

    public static bool IsRemote => !string.IsNullOrWhiteSpace(BaseUrl);

    public static string Mode => IsRemote ? $"remoto ({BaseUrl})" : "em memória (WebApplicationFactory)";

    public static HttpClient CreateClient()
    {
        if (IsRemote)
            return new HttpClient { BaseAddress = new Uri(BaseUrl!.TrimEnd('/') + "/"), Timeout = TimeSpan.FromSeconds(30) };

        lock (Lock)
        {
            _factory ??= new InMemoryEcoTrackerFactory();
        }

        return _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    public static void Shutdown()
    {
        lock (Lock)
        {
            _factory?.Dispose();
            _factory = null;
        }
    }

    private sealed class InMemoryEcoTrackerFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName = "EcoTrackerBdd_" + Guid.NewGuid();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning));
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<EcoTrackerDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<EcoTrackerDbContext>(options => options.UseInMemoryDatabase(_dbName));
            });
        }
    }
}
