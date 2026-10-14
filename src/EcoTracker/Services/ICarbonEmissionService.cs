using EcoTracker.ViewModels;

namespace EcoTracker.Services;

public interface ICarbonEmissionService
{
    Task<PagedResult<CarbonEmissionResponseViewModel>> GetAllAsync(int page, int pageSize, string? company, string? sector, int? year);
    Task<CarbonEmissionResponseViewModel?> GetByIdAsync(int id);
    Task<CarbonEmissionResponseViewModel> CreateAsync(CarbonEmissionCreateViewModel model);
    Task<CarbonEmissionResponseViewModel?> UpdateAsync(int id, CarbonEmissionUpdateViewModel model);
    Task<bool> DeleteAsync(int id);
}
