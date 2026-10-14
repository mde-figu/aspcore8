using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EcoTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarbonEmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Sector = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmissionTonsCO2 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    OffsetTonsCO2 = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ReferenceYear = table.Column<int>(type: "int", nullable: false),
                    ReferenceMonth = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarbonEmissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ComplianceAudits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AuditorName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    NormReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AuditDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ComplianceScore = table.Column<int>(type: "int", nullable: false),
                    Findings = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CorrectiveActions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    NextAuditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplianceAudits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnvironmentalAlerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AlertType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RelatedLicenseNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvironmentalAlerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EnvironmentalLicenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LicenseType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IssuingAuthority = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvironmentalLicenses", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CarbonEmissions",
                columns: new[] { "Id", "CompanyName", "CreatedAt", "Description", "EmissionTonsCO2", "OffsetTonsCO2", "ReferenceMonth", "ReferenceYear", "Sector", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "EcoCorp Brasil", new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Emissões do primeiro trimestre", 1500.5m, 300.0m, 1, 2024, "Energia", null },
                    { 2, "GreenTech Ltda", new DateTime(2024, 2, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Emissões do datacenter", 250.0m, 100.0m, 2, 2024, "Tecnologia", null },
                    { 3, "AgroSustentavel SA", new DateTime(2024, 3, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Emissões agrícolas Q1", 3200.0m, 1500.0m, 3, 2024, "Agronegócio", null }
                });

            migrationBuilder.InsertData(
                table: "ComplianceAudits",
                columns: new[] { "Id", "AuditDate", "AuditorName", "CompanyName", "ComplianceScore", "CorrectiveActions", "CreatedAt", "Findings", "NextAuditDate", "NormReference", "Result", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 2, 20, 0, 0, 0, 0, DateTimeKind.Utc), "João Silva", "EcoCorp Brasil", 92, null, new DateTime(2024, 2, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Conformidade elevada com pequenas observações", new DateTime(2025, 2, 20, 0, 0, 0, 0, DateTimeKind.Utc), "ISO 14001:2015", "Approved", null },
                    { 2, new DateTime(2024, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Maria Oliveira", "AgroSustentavel SA", 75, "Implementar sistema de monitoramento contínuo", new DateTime(2024, 4, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Necessita melhorias no controle de emissões", new DateTime(2024, 10, 10, 0, 0, 0, 0, DateTimeKind.Utc), "ISO 14064:2018", "ConditionalApproval", null }
                });

            migrationBuilder.InsertData(
                table: "EnvironmentalAlerts",
                columns: new[] { "Id", "AlertType", "CompanyName", "CreatedAt", "IsResolved", "Message", "RelatedLicenseNumber", "ResolutionNotes", "ResolvedAt", "Severity" },
                values: new object[,]
                {
                    { 1, "LicenseExpiration", "EcoCorp Brasil", new DateTime(2024, 12, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Licença LIC-2024-001 expira em 30 dias", "LIC-2024-001", null, null, "High" },
                    { 2, "EmissionThreshold", "AgroSustentavel SA", new DateTime(2024, 3, 10, 0, 0, 0, 0, DateTimeKind.Utc), false, "Emissão líquida acima do limite permitido para o setor", null, null, null, "Critical" }
                });

            migrationBuilder.InsertData(
                table: "EnvironmentalLicenses",
                columns: new[] { "Id", "CompanyName", "CreatedAt", "ExpirationDate", "IssueDate", "IssuingAuthority", "LicenseNumber", "LicenseType", "Notes", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "EcoCorp Brasil", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "IBAMA", "LIC-2024-001", "Licença de Operação", null, "Active", null },
                    { 2, "GreenTech Ltda", new DateTime(2024, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2024, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), "CETESB", "LIC-2024-002", "Licença Prévia", null, "Active", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarbonEmissions_CompanyName",
                table: "CarbonEmissions",
                column: "CompanyName");

            migrationBuilder.CreateIndex(
                name: "IX_CarbonEmissions_ReferenceYear_ReferenceMonth",
                table: "CarbonEmissions",
                columns: new[] { "ReferenceYear", "ReferenceMonth" });

            migrationBuilder.CreateIndex(
                name: "IX_CarbonEmissions_Sector",
                table: "CarbonEmissions",
                column: "Sector");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceAudits_AuditDate",
                table: "ComplianceAudits",
                column: "AuditDate");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceAudits_CompanyName",
                table: "ComplianceAudits",
                column: "CompanyName");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceAudits_NormReference",
                table: "ComplianceAudits",
                column: "NormReference");

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentalAlerts_CompanyName",
                table: "EnvironmentalAlerts",
                column: "CompanyName");

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentalAlerts_IsResolved",
                table: "EnvironmentalAlerts",
                column: "IsResolved");

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentalAlerts_Severity",
                table: "EnvironmentalAlerts",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentalLicenses_CompanyName",
                table: "EnvironmentalLicenses",
                column: "CompanyName");

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentalLicenses_ExpirationDate",
                table: "EnvironmentalLicenses",
                column: "ExpirationDate");

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentalLicenses_LicenseNumber",
                table: "EnvironmentalLicenses",
                column: "LicenseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentalLicenses_Status",
                table: "EnvironmentalLicenses",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarbonEmissions");

            migrationBuilder.DropTable(
                name: "ComplianceAudits");

            migrationBuilder.DropTable(
                name: "EnvironmentalAlerts");

            migrationBuilder.DropTable(
                name: "EnvironmentalLicenses");
        }
    }
}
