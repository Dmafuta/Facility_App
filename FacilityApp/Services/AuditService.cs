using FacilityApp.Data;
using FacilityApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FacilityApp.Services;

public class AuditService : IAuditService
{
    private readonly AppDbContext _context;
    private readonly TenantContext _tenantCtx;

    public AuditService(AppDbContext context, TenantContext tenantCtx)
    {
        _context   = context;
        _tenantCtx = tenantCtx;
    }

    public async Task LogAsync(string action, string entityType, string? entityId = null,
        string? details = null, string? userId = null, string? userName = null)
    {
        if (_tenantCtx.TenantId == Guid.Empty) return;

        _context.AuditLogs.Add(new AuditLog
        {
            TenantId   = _tenantCtx.TenantId,
            UserId     = userId,
            UserName   = userName ?? "System",
            Action     = action,
            EntityType = entityType,
            EntityId   = entityId,
            Details    = details,
            CreatedAt  = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
    }

    public async Task<(List<AuditLog> Items, int Total)> GetLogsAsync(
        string? search = null, int page = 1, int pageSize = 50)
    {
        var q = _context.AuditLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            q = q.Where(a =>
                a.UserName.ToLower().Contains(s)     ||
                a.Action.ToLower().Contains(s)        ||
                a.EntityType.ToLower().Contains(s)    ||
                (a.Details != null && a.Details.ToLower().Contains(s)));
        }

        var total = await q.CountAsync();
        var items = await q
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}
