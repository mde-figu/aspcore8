using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoTracker.Models;

[Table("EnvironmentalLicenses")]
public class EnvironmentalLicense
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LicenseNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LicenseType { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string IssuingAuthority { get; set; } = string.Empty;

    [Required]
    public DateTime IssueDate { get; set; }

    [Required]
    public DateTime ExpirationDate { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Active";

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
