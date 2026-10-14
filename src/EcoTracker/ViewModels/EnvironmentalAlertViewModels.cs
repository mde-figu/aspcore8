using System.ComponentModel.DataAnnotations;

namespace EcoTracker.ViewModels;

public class EnvironmentalAlertCreateViewModel
{
    [Required(ErrorMessage = "O nome da empresa é obrigatório.")]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "O tipo de alerta é obrigatório.")]
    [MaxLength(100)]
    public string AlertType { get; set; } = string.Empty;

    [Required(ErrorMessage = "A severidade é obrigatória.")]
    [RegularExpression("^(Low|Medium|High|Critical)$", ErrorMessage = "Severidade deve ser: Low, Medium, High ou Critical.")]
    public string Severity { get; set; } = "Medium";

    [Required(ErrorMessage = "A mensagem é obrigatória.")]
    [MaxLength(500)]
    public string Message { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? RelatedLicenseNumber { get; set; }
}

public class EnvironmentalAlertResolveViewModel
{
    [Required(ErrorMessage = "As notas de resolução são obrigatórias.")]
    [MaxLength(500)]
    public string ResolutionNotes { get; set; } = string.Empty;
}

public class EnvironmentalAlertResponseViewModel
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string AlertType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? RelatedLicenseNumber { get; set; }
    public bool IsResolved { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolutionNotes { get; set; }
    public DateTime CreatedAt { get; set; }
}
