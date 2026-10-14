using System.ComponentModel.DataAnnotations;

namespace EcoTracker.ViewModels;

public class EnvironmentalLicenseCreateViewModel
{
    [Required(ErrorMessage = "O nome da empresa é obrigatório.")]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "O número da licença é obrigatório.")]
    [MaxLength(100)]
    public string LicenseNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "O tipo de licença é obrigatório.")]
    [MaxLength(100)]
    public string LicenseType { get; set; } = string.Empty;

    [Required(ErrorMessage = "A autoridade emissora é obrigatória.")]
    [MaxLength(150)]
    public string IssuingAuthority { get; set; } = string.Empty;

    [Required(ErrorMessage = "A data de emissão é obrigatória.")]
    public DateTime IssueDate { get; set; }

    [Required(ErrorMessage = "A data de validade é obrigatória.")]
    public DateTime ExpirationDate { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}

public class EnvironmentalLicenseUpdateViewModel
{
    [MaxLength(50)]
    public string? Status { get; set; }

    public DateTime? ExpirationDate { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}

public class EnvironmentalLicenseResponseViewModel
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string LicenseType { get; set; } = string.Empty;
    public string IssuingAuthority { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int DaysUntilExpiration => (ExpirationDate - DateTime.UtcNow).Days;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
