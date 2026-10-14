using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoTracker.Models;

[Table("EnvironmentalAlerts")]
public class EnvironmentalAlert
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string AlertType { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Severity { get; set; } = "Medium";

    [Required]
    [MaxLength(500)]
    public string Message { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? RelatedLicenseNumber { get; set; }

    public bool IsResolved { get; set; }

    public DateTime? ResolvedAt { get; set; }

    [MaxLength(500)]
    public string? ResolutionNotes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
