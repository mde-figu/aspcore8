using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoTracker.Models;

[Table("ComplianceAudits")]
public class ComplianceAudit
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string AuditorName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string NormReference { get; set; } = string.Empty;

    [Required]
    public DateTime AuditDate { get; set; }

    [Required]
    [MaxLength(50)]
    public string Result { get; set; } = "Pending";

    [Range(0, 100)]
    public int ComplianceScore { get; set; }

    [MaxLength(2000)]
    public string? Findings { get; set; }

    [MaxLength(2000)]
    public string? CorrectiveActions { get; set; }

    public DateTime? NextAuditDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
