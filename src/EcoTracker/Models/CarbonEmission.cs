using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoTracker.Models;

[Table("CarbonEmissions")]
public class CarbonEmission
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Sector { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,4)")]
    public decimal EmissionTonsCO2 { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,4)")]
    public decimal OffsetTonsCO2 { get; set; }

    [Required]
    public int ReferenceYear { get; set; }

    [Required]
    public int ReferenceMonth { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
