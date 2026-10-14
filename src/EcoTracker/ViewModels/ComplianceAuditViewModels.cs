using System.ComponentModel.DataAnnotations;

namespace EcoTracker.ViewModels;

public class ComplianceAuditCreateViewModel
{
    [Required(ErrorMessage = "O nome da empresa é obrigatório.")]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome do auditor é obrigatório.")]
    [MaxLength(150)]
    public string AuditorName { get; set; } = string.Empty;

    [Required(ErrorMessage = "A referência da norma é obrigatória.")]
    [MaxLength(100)]
    public string NormReference { get; set; } = string.Empty;

    [Required(ErrorMessage = "A data da auditoria é obrigatória.")]
    public DateTime AuditDate { get; set; }

    [Range(0, 100, ErrorMessage = "O score deve estar entre 0 e 100.")]
    public int ComplianceScore { get; set; }

    [MaxLength(2000)]
    public string? Findings { get; set; }

    [MaxLength(2000)]
    public string? CorrectiveActions { get; set; }

    public DateTime? NextAuditDate { get; set; }
}

public class ComplianceAuditUpdateViewModel
{
    [MaxLength(50)]
    public string? Result { get; set; }

    [Range(0, 100, ErrorMessage = "O score deve estar entre 0 e 100.")]
    public int? ComplianceScore { get; set; }

    [MaxLength(2000)]
    public string? Findings { get; set; }

    [MaxLength(2000)]
    public string? CorrectiveActions { get; set; }

    public DateTime? NextAuditDate { get; set; }
}

public class ComplianceAuditResponseViewModel
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string AuditorName { get; set; } = string.Empty;
    public string NormReference { get; set; } = string.Empty;
    public DateTime AuditDate { get; set; }
    public string Result { get; set; } = string.Empty;
    public int ComplianceScore { get; set; }
    public string? Findings { get; set; }
    public string? CorrectiveActions { get; set; }
    public DateTime? NextAuditDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
