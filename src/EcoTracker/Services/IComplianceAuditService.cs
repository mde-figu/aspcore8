using EcoTracker.ViewModels;

namespace EcoTracker.Services;

public interface IComplianceAuditService
{
    Task<PagedResult<ComplianceAuditResponseViewModel>> GetAllAsync(int page, int pageSize, string? company, string? norm);
    Task<ComplianceAuditResponseViewModel?> GetByIdAsync(int id);
    Task<ComplianceAuditResponseViewModel> CreateAsync(ComplianceAuditCreateViewModel model);
    Task<ComplianceAuditResponseViewModel?> UpdateAsync(int id, ComplianceAuditUpdateViewModel model);
    Task<bool> DeleteAsync(int id);
}
