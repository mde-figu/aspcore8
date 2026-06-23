using EcoTracker.ViewModels;

namespace EcoTracker.Services;

public interface IEnvironmentalAlertService
{
    Task<PagedResult<EnvironmentalAlertResponseViewModel>> GetAllAsync(int page, int pageSize, string? company, string? severity, bool? resolved);
    Task<EnvironmentalAlertResponseViewModel?> GetByIdAsync(int id);
    Task<EnvironmentalAlertResponseViewModel> CreateAsync(EnvironmentalAlertCreateViewModel model);
    Task<EnvironmentalAlertResponseViewModel?> ResolveAsync(int id, EnvironmentalAlertResolveViewModel model);
    Task<bool> DeleteAsync(int id);
}
