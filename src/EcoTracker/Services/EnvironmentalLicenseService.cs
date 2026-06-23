using EcoTracker.Data;
using EcoTracker.Models;
using EcoTracker.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EcoTracker.Services;

public class EnvironmentalLicenseService : IEnvironmentalLicenseService
{
    private readonly EcoTrackerDbContext _context;

    public EnvironmentalLicenseService(EcoTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<EnvironmentalLicenseResponseViewModel>> GetAllAsync(
        int page, int pageSize, string? company, string? status)
    {
        var query = _context.EnvironmentalLicenses.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(company))
            query = query.Where(e => e.CompanyName.Contains(company));

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(e => e.Status == status);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(e => e.ExpirationDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => MapToResponse(e))
            .ToListAsync();

        return new PagedResult<EnvironmentalLicenseResponseViewModel>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<EnvironmentalLicenseResponseViewModel?> GetByIdAsync(int id)
    {
        var entity = await _context.EnvironmentalLicenses.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<IReadOnlyList<EnvironmentalLicenseResponseViewModel>> GetExpiringAsync(int daysThreshold)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);
        return await _context.EnvironmentalLicenses
            .AsNoTracking()
            .Where(e => e.Status == "Active" && e.ExpirationDate <= thresholdDate)
            .OrderBy(e => e.ExpirationDate)
            .Select(e => MapToResponse(e))
            .ToListAsync();
    }

    public async Task<EnvironmentalLicenseResponseViewModel> CreateAsync(EnvironmentalLicenseCreateViewModel model)
    {
        var entity = new EnvironmentalLicense
        {
            CompanyName = model.CompanyName,
            LicenseNumber = model.LicenseNumber,
            LicenseType = model.LicenseType,
            IssuingAuthority = model.IssuingAuthority,
            IssueDate = model.IssueDate,
            ExpirationDate = model.ExpirationDate,
            Status = "Active",
            Notes = model.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _context.EnvironmentalLicenses.Add(entity);
        await _context.SaveChangesAsync();

        return MapToResponse(entity);
    }

    public async Task<EnvironmentalLicenseResponseViewModel?> UpdateAsync(int id, EnvironmentalLicenseUpdateViewModel model)
    {
        var entity = await _context.EnvironmentalLicenses.FindAsync(id);
        if (entity is null) return null;

        if (model.Status is not null)
            entity.Status = model.Status;

        if (model.ExpirationDate.HasValue)
            entity.ExpirationDate = model.ExpirationDate.Value;

        if (model.Notes is not null)
            entity.Notes = model.Notes;

        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToResponse(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.EnvironmentalLicenses.FindAsync(id);
        if (entity is null) return false;

        _context.EnvironmentalLicenses.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private static EnvironmentalLicenseResponseViewModel MapToResponse(EnvironmentalLicense e) => new()
    {
        Id = e.Id,
        CompanyName = e.CompanyName,
        LicenseNumber = e.LicenseNumber,
        LicenseType = e.LicenseType,
        IssuingAuthority = e.IssuingAuthority,
        IssueDate = e.IssueDate,
        ExpirationDate = e.ExpirationDate,
        Status = e.Status,
        Notes = e.Notes,
        CreatedAt = e.CreatedAt
    };
}
