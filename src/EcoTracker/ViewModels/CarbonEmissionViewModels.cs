using System.ComponentModel.DataAnnotations;

namespace EcoTracker.ViewModels;

public class CarbonEmissionCreateViewModel
{
    [Required(ErrorMessage = "O nome da empresa é obrigatório.")]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "O setor é obrigatório.")]
    [MaxLength(100)]
    public string Sector { get; set; } = string.Empty;

    [Required(ErrorMessage = "A emissão em toneladas de CO2 é obrigatória.")]
    [Range(0, double.MaxValue, ErrorMessage = "A emissão deve ser um valor positivo.")]
    public decimal EmissionTonsCO2 { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "A compensação deve ser um valor positivo.")]
    public decimal OffsetTonsCO2 { get; set; }

    [Required(ErrorMessage = "O ano de referência é obrigatório.")]
    [Range(2000, 2100)]
    public int ReferenceYear { get; set; }

    [Required(ErrorMessage = "O mês de referência é obrigatório.")]
    [Range(1, 12)]
    public int ReferenceMonth { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}

public class CarbonEmissionUpdateViewModel
{
    [Range(0, double.MaxValue, ErrorMessage = "A emissão deve ser um valor positivo.")]
    public decimal? EmissionTonsCO2 { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "A compensação deve ser um valor positivo.")]
    public decimal? OffsetTonsCO2 { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}

public class CarbonEmissionResponseViewModel
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public decimal EmissionTonsCO2 { get; set; }
    public decimal OffsetTonsCO2 { get; set; }
    public decimal NetEmission => EmissionTonsCO2 - OffsetTonsCO2;
    public int ReferenceYear { get; set; }
    public int ReferenceMonth { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
