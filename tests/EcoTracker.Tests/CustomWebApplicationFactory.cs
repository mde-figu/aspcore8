using EcoTracker.Data;
using EcoTracker.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EcoTracker.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = "EcoTrackerTestDb_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<EcoTrackerDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<EcoTrackerDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<EcoTrackerDbContext>();
            db.Database.EnsureCreated();
            SeedTestData(db);
        });
    }

    private static void SeedTestData(EcoTrackerDbContext db)
    {
        if (db.CarbonEmissions.Any()) return;

        db.CarbonEmissions.AddRange(
            new CarbonEmission { Id = 1, CompanyName = "EcoCorp Brasil", Sector = "Energia", EmissionTonsCO2 = 1500.5m, OffsetTonsCO2 = 300.0m, ReferenceYear = 2024, ReferenceMonth = 1, Description = "Emissões Q1", CreatedAt = DateTime.UtcNow },
            new CarbonEmission { Id = 2, CompanyName = "GreenTech Ltda", Sector = "Tecnologia", EmissionTonsCO2 = 250.0m, OffsetTonsCO2 = 100.0m, ReferenceYear = 2024, ReferenceMonth = 2, CreatedAt = DateTime.UtcNow }
        );

        db.EnvironmentalLicenses.AddRange(
            new EnvironmentalLicense { Id = 1, CompanyName = "EcoCorp Brasil", LicenseNumber = "LIC-2024-001", LicenseType = "Licença de Operação", IssuingAuthority = "IBAMA", IssueDate = DateTime.UtcNow.AddYears(-1), ExpirationDate = DateTime.UtcNow.AddMonths(6), Status = "Active", CreatedAt = DateTime.UtcNow },
            new EnvironmentalLicense { Id = 2, CompanyName = "GreenTech Ltda", LicenseNumber = "LIC-2024-002", LicenseType = "Licença Prévia", IssuingAuthority = "CETESB", IssueDate = DateTime.UtcNow.AddMonths(-6), ExpirationDate = DateTime.UtcNow.AddYears(2), Status = "Active", CreatedAt = DateTime.UtcNow }
        );

        db.ComplianceAudits.AddRange(
            new ComplianceAudit { Id = 1, CompanyName = "EcoCorp Brasil", AuditorName = "João Silva", NormReference = "ISO 14001:2015", AuditDate = DateTime.UtcNow.AddMonths(-3), Result = "Approved", ComplianceScore = 92, CreatedAt = DateTime.UtcNow },
            new ComplianceAudit { Id = 2, CompanyName = "AgroSustentavel SA", AuditorName = "Maria Oliveira", NormReference = "ISO 14064:2018", AuditDate = DateTime.UtcNow.AddMonths(-1), Result = "ConditionalApproval", ComplianceScore = 75, CreatedAt = DateTime.UtcNow }
        );

        db.EnvironmentalAlerts.AddRange(
            new EnvironmentalAlert { Id = 1, CompanyName = "EcoCorp Brasil", AlertType = "LicenseExpiration", Severity = "High", Message = "Licença expira em 30 dias", IsResolved = false, CreatedAt = DateTime.UtcNow },
            new EnvironmentalAlert { Id = 2, CompanyName = "AgroSustentavel SA", AlertType = "EmissionThreshold", Severity = "Critical", Message = "Emissão acima do limite", IsResolved = false, CreatedAt = DateTime.UtcNow }
        );

        db.SaveChanges();
    }
}
