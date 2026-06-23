using EcoTracker.Data;
using EcoTracker.Models;
using EcoTracker.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EcoTracker.Services;

public class EnvironmentalAlertService : IEnvironmentalAlertService
{
    private readonly EcoTrackerDbContext _context;

    public EnvironmentalAlertService(EcoTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<EnvironmentalAlertResponseViewModel>> GetAllAsync(
        int page, int pageSize, string? company, string? severity, bool? resolved)
    {
        var query = _context.EnvironmentalAlerts.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(company))
            query = query.Where(e => e.CompanyName.Contains(company));

        if (!string.IsNullOrWhiteSpace(severity))
            query = query.Where(e => e.Severity == severity);

        if (resolved.HasValue)
            query = query.Where(e => e.IsResolved == resolved.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => MapToResponse(e))
            .ToListAsync();

        return new PagedResult<EnvironmentalAlertResponseViewModel>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<EnvironmentalAlertResponseViewModel?> GetByIdAsync(int id)
    {
        var entity = await _context.EnvironmentalAlerts.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<EnvironmentalAlertResponseViewModel> CreateAsync(EnvironmentalAlertCreateViewModel model)
    {
        var entity = new EnvironmentalAlert
        {
            CompanyName = model.CompanyName,
            AlertType = model.AlertType,
            Severity = model.Severity,
            Message = model.Message,
            RelatedLicenseNumber = model.RelatedLicenseNumber,
            IsResolved = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.EnvironmentalAlerts.Add(entity);
        await _context.SaveChangesAsync();

        return MapToResponse(entity);
    }

    public async Task<EnvironmentalAlertResponseViewModel?> ResolveAsync(int id, EnvironmentalAlertResolveViewModel model)
    {
        var entity = await _context.EnvironmentalAlerts.FindAsync(id);
        if (entity is null) return null;

        entity.IsResolved = true;
        entity.ResolvedAt = DateTime.UtcNow;
        entity.ResolutionNotes = model.ResolutionNotes;

        await _context.SaveChangesAsync();

        return MapToResponse(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.EnvironmentalAlerts.FindAsync(id);
        if (entity is null) return false;

        _context.EnvironmentalAlerts.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private static EnvironmentalAlertResponseViewModel MapToResponse(EnvironmentalAlert e) => new()
    {
        Id = e.Id,
        CompanyName = e.CompanyName,
        AlertType = e.AlertType,
        Severity = e.Severity,
        Message = e.Message,
        RelatedLicenseNumber = e.RelatedLicenseNumber,
        IsResolved = e.IsResolved,
        ResolvedAt = e.ResolvedAt,
        ResolutionNotes = e.ResolutionNotes,
        CreatedAt = e.CreatedAt
    };
}
