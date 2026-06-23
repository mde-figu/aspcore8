using EcoTracker.Data;
using EcoTracker.Models;
using EcoTracker.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EcoTracker.Services;

public class CarbonEmissionService : ICarbonEmissionService
{
    private readonly EcoTrackerDbContext _context;

    public CarbonEmissionService(EcoTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<CarbonEmissionResponseViewModel>> GetAllAsync(
        int page, int pageSize, string? company, string? sector, int? year)
    {
        var query = _context.CarbonEmissions.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(company))
            query = query.Where(e => e.CompanyName.Contains(company));

        if (!string.IsNullOrWhiteSpace(sector))
            query = query.Where(e => e.Sector.Contains(sector));

        if (year.HasValue)
            query = query.Where(e => e.ReferenceYear == year.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(e => e.ReferenceYear)
            .ThenByDescending(e => e.ReferenceMonth)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => MapToResponse(e))
            .ToListAsync();

        return new PagedResult<CarbonEmissionResponseViewModel>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<CarbonEmissionResponseViewModel?> GetByIdAsync(int id)
    {
        var entity = await _context.CarbonEmissions.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<CarbonEmissionResponseViewModel> CreateAsync(CarbonEmissionCreateViewModel model)
    {
        var entity = new CarbonEmission
        {
            CompanyName = model.CompanyName,
            Sector = model.Sector,
            EmissionTonsCO2 = model.EmissionTonsCO2,
            OffsetTonsCO2 = model.OffsetTonsCO2,
            ReferenceYear = model.ReferenceYear,
            ReferenceMonth = model.ReferenceMonth,
            Description = model.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.CarbonEmissions.Add(entity);
        await _context.SaveChangesAsync();

        return MapToResponse(entity);
    }

    public async Task<CarbonEmissionResponseViewModel?> UpdateAsync(int id, CarbonEmissionUpdateViewModel model)
    {
        var entity = await _context.CarbonEmissions.FindAsync(id);
        if (entity is null) return null;

        if (model.EmissionTonsCO2.HasValue)
            entity.EmissionTonsCO2 = model.EmissionTonsCO2.Value;

        if (model.OffsetTonsCO2.HasValue)
            entity.OffsetTonsCO2 = model.OffsetTonsCO2.Value;

        if (model.Description is not null)
            entity.Description = model.Description;

        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToResponse(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.CarbonEmissions.FindAsync(id);
        if (entity is null) return false;

        _context.CarbonEmissions.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private static CarbonEmissionResponseViewModel MapToResponse(CarbonEmission e) => new()
    {
        Id = e.Id,
        CompanyName = e.CompanyName,
        Sector = e.Sector,
        EmissionTonsCO2 = e.EmissionTonsCO2,
        OffsetTonsCO2 = e.OffsetTonsCO2,
        ReferenceYear = e.ReferenceYear,
        ReferenceMonth = e.ReferenceMonth,
        Description = e.Description,
        CreatedAt = e.CreatedAt
    };
}
