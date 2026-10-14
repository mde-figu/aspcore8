using EcoTracker.Data;
using EcoTracker.Models;
using EcoTracker.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EcoTracker.Services;

public class ComplianceAuditService : IComplianceAuditService
{
    private readonly EcoTrackerDbContext _context;

    public ComplianceAuditService(EcoTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ComplianceAuditResponseViewModel>> GetAllAsync(
        int page, int pageSize, string? company, string? norm)
    {
        var query = _context.ComplianceAudits.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(company))
            query = query.Where(e => e.CompanyName.Contains(company));

        if (!string.IsNullOrWhiteSpace(norm))
            query = query.Where(e => e.NormReference.Contains(norm));

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(e => e.AuditDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => MapToResponse(e))
            .ToListAsync();

        return new PagedResult<ComplianceAuditResponseViewModel>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ComplianceAuditResponseViewModel?> GetByIdAsync(int id)
    {
        var entity = await _context.ComplianceAudits.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        return entity is null ? null : MapToResponse(entity);
    }

    public async Task<ComplianceAuditResponseViewModel> CreateAsync(ComplianceAuditCreateViewModel model)
    {
        var entity = new ComplianceAudit
        {
            CompanyName = model.CompanyName,
            AuditorName = model.AuditorName,
            NormReference = model.NormReference,
            AuditDate = model.AuditDate,
            ComplianceScore = model.ComplianceScore,
            Findings = model.Findings,
            CorrectiveActions = model.CorrectiveActions,
            NextAuditDate = model.NextAuditDate,
            Result = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.ComplianceAudits.Add(entity);
        await _context.SaveChangesAsync();

        return MapToResponse(entity);
    }

    public async Task<ComplianceAuditResponseViewModel?> UpdateAsync(int id, ComplianceAuditUpdateViewModel model)
    {
        var entity = await _context.ComplianceAudits.FindAsync(id);
        if (entity is null) return null;

        if (model.Result is not null)
            entity.Result = model.Result;

        if (model.ComplianceScore.HasValue)
            entity.ComplianceScore = model.ComplianceScore.Value;

        if (model.Findings is not null)
            entity.Findings = model.Findings;

        if (model.CorrectiveActions is not null)
            entity.CorrectiveActions = model.CorrectiveActions;

        if (model.NextAuditDate.HasValue)
            entity.NextAuditDate = model.NextAuditDate.Value;

        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return MapToResponse(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.ComplianceAudits.FindAsync(id);
        if (entity is null) return false;

        _context.ComplianceAudits.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    private static ComplianceAuditResponseViewModel MapToResponse(ComplianceAudit e) => new()
    {
        Id = e.Id,
        CompanyName = e.CompanyName,
        AuditorName = e.AuditorName,
        NormReference = e.NormReference,
        AuditDate = e.AuditDate,
        Result = e.Result,
        ComplianceScore = e.ComplianceScore,
        Findings = e.Findings,
        CorrectiveActions = e.CorrectiveActions,
        NextAuditDate = e.NextAuditDate,
        CreatedAt = e.CreatedAt
    };
}
