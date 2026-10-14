using EcoTracker.ViewModels;

namespace EcoTracker.Services;

public interface IEnvironmentalLicenseService
{
    Task<PagedResult<EnvironmentalLicenseResponseViewModel>> GetAllAsync(int page, int pageSize, string? company, string? status);
    Task<EnvironmentalLicenseResponseViewModel?> GetByIdAsync(int id);
    Task<IReadOnlyList<EnvironmentalLicenseResponseViewModel>> GetExpiringAsync(int daysThreshold);
    Task<EnvironmentalLicenseResponseViewModel> CreateAsync(EnvironmentalLicenseCreateViewModel model);
    Task<EnvironmentalLicenseResponseViewModel?> UpdateAsync(int id, EnvironmentalLicenseUpdateViewModel model);
    Task<bool> DeleteAsync(int id);
}
