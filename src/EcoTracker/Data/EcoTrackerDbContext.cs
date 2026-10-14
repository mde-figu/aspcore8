using EcoTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace EcoTracker.Data;

public class EcoTrackerDbContext : DbContext
{
    public EcoTrackerDbContext(DbContextOptions<EcoTrackerDbContext> options) : base(options) { }

    public DbSet<CarbonEmission> CarbonEmissions => Set<CarbonEmission>();
    public DbSet<EnvironmentalLicense> EnvironmentalLicenses => Set<EnvironmentalLicense>();
    public DbSet<ComplianceAudit> ComplianceAudits => Set<ComplianceAudit>();
    public DbSet<EnvironmentalAlert> EnvironmentalAlerts => Set<EnvironmentalAlert>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CarbonEmission>(entity =>
        {
            entity.HasIndex(e => e.CompanyName);
            entity.HasIndex(e => new { e.ReferenceYear, e.ReferenceMonth });
            entity.HasIndex(e => e.Sector);
        });

        modelBuilder.Entity<EnvironmentalLicense>(entity =>
        {
            entity.HasIndex(e => e.LicenseNumber).IsUnique();
            entity.HasIndex(e => e.CompanyName);
            entity.HasIndex(e => e.ExpirationDate);
            entity.HasIndex(e => e.Status);
        });

        modelBuilder.Entity<ComplianceAudit>(entity =>
        {
            entity.HasIndex(e => e.CompanyName);
            entity.HasIndex(e => e.AuditDate);
            entity.HasIndex(e => e.NormReference);
        });

        modelBuilder.Entity<EnvironmentalAlert>(entity =>
        {
            entity.HasIndex(e => e.CompanyName);
            entity.HasIndex(e => e.Severity);
            entity.HasIndex(e => e.IsResolved);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CarbonEmission>().HasData(
            new CarbonEmission { Id = 1, CompanyName = "EcoCorp Brasil", Sector = "Energia", EmissionTonsCO2 = 1500.5m, OffsetTonsCO2 = 300.0m, ReferenceYear = 2024, ReferenceMonth = 1, Description = "Emissões do primeiro trimestre", CreatedAt = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc) },
            new CarbonEmission { Id = 2, CompanyName = "GreenTech Ltda", Sector = "Tecnologia", EmissionTonsCO2 = 250.0m, OffsetTonsCO2 = 100.0m, ReferenceYear = 2024, ReferenceMonth = 2, Description = "Emissões do datacenter", CreatedAt = new DateTime(2024, 2, 10, 0, 0, 0, DateTimeKind.Utc) },
            new CarbonEmission { Id = 3, CompanyName = "AgroSustentavel SA", Sector = "Agronegócio", EmissionTonsCO2 = 3200.0m, OffsetTonsCO2 = 1500.0m, ReferenceYear = 2024, ReferenceMonth = 3, Description = "Emissões agrícolas Q1", CreatedAt = new DateTime(2024, 3, 5, 0, 0, 0, DateTimeKind.Utc) }
        );

        modelBuilder.Entity<EnvironmentalLicense>().HasData(
            new EnvironmentalLicense { Id = 1, CompanyName = "EcoCorp Brasil", LicenseNumber = "LIC-2024-001", LicenseType = "Licença de Operação", IssuingAuthority = "IBAMA", IssueDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), ExpirationDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new EnvironmentalLicense { Id = 2, CompanyName = "GreenTech Ltda", LicenseNumber = "LIC-2024-002", LicenseType = "Licença Prévia", IssuingAuthority = "CETESB", IssueDate = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc), ExpirationDate = new DateTime(2026, 3, 15, 0, 0, 0, DateTimeKind.Utc), Status = "Active", CreatedAt = new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc) }
        );

        modelBuilder.Entity<ComplianceAudit>().HasData(
            new ComplianceAudit { Id = 1, CompanyName = "EcoCorp Brasil", AuditorName = "João Silva", NormReference = "ISO 14001:2015", AuditDate = new DateTime(2024, 2, 20, 0, 0, 0, DateTimeKind.Utc), Result = "Approved", ComplianceScore = 92, Findings = "Conformidade elevada com pequenas observações", NextAuditDate = new DateTime(2025, 2, 20, 0, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(2024, 2, 20, 0, 0, 0, DateTimeKind.Utc) },
            new ComplianceAudit { Id = 2, CompanyName = "AgroSustentavel SA", AuditorName = "Maria Oliveira", NormReference = "ISO 14064:2018", AuditDate = new DateTime(2024, 4, 10, 0, 0, 0, DateTimeKind.Utc), Result = "ConditionalApproval", ComplianceScore = 75, Findings = "Necessita melhorias no controle de emissões", CorrectiveActions = "Implementar sistema de monitoramento contínuo", NextAuditDate = new DateTime(2024, 10, 10, 0, 0, 0, DateTimeKind.Utc), CreatedAt = new DateTime(2024, 4, 10, 0, 0, 0, DateTimeKind.Utc) }
        );

        modelBuilder.Entity<EnvironmentalAlert>().HasData(
            new EnvironmentalAlert { Id = 1, CompanyName = "EcoCorp Brasil", AlertType = "LicenseExpiration", Severity = "High", Message = "Licença LIC-2024-001 expira em 30 dias", RelatedLicenseNumber = "LIC-2024-001", IsResolved = false, CreatedAt = new DateTime(2024, 12, 1, 0, 0, 0, DateTimeKind.Utc) },
            new EnvironmentalAlert { Id = 2, CompanyName = "AgroSustentavel SA", AlertType = "EmissionThreshold", Severity = "Critical", Message = "Emissão líquida acima do limite permitido para o setor", IsResolved = false, CreatedAt = new DateTime(2024, 3, 10, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
