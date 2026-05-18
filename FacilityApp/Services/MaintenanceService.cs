using FacilityApp.Data;
using FacilityApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FacilityApp.Services;

public class MaintenanceService : IMaintenanceService
{
    private readonly AppDbContext _db;
    private readonly TenantContext _tenantCtx;

    public MaintenanceService(AppDbContext db, TenantContext tenantCtx)
    {
        _db        = db;
        _tenantCtx = tenantCtx;
    }

    public async Task<List<MaintenanceRequest>> GetForResidentAsync(string residentId)
    {
        return await _db.MaintenanceRequests
            .Include(m => m.Unit)
            .Where(m => m.ResidentId == residentId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<MaintenanceRequest>> GetAllAsync(MaintenanceStatus? status = null)
    {
        var query = _db.MaintenanceRequests
            .Include(m => m.Resident)
            .Include(m => m.Unit)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(m => m.Status == status.Value);

        return await query.OrderByDescending(m => m.CreatedAt).ToListAsync();
    }

    public async Task<int> GetOpenCountAsync()
    {
        return await _db.MaintenanceRequests
            .CountAsync(m => m.Status == MaintenanceStatus.Open || m.Status == MaintenanceStatus.InProgress);
    }

    public async Task<MaintenanceRequest> SubmitAsync(string residentId, Guid? unitId, string title, string description, MaintenanceCategory category, MaintenancePriority priority)
    {
        var request = new MaintenanceRequest
        {
            TenantId    = _tenantCtx.TenantId,
            ResidentId  = residentId,
            UnitId      = unitId,
            Title       = title.Trim(),
            Description = description.Trim(),
            Category    = category,
            Priority    = priority
        };

        _db.MaintenanceRequests.Add(request);
        await _db.SaveChangesAsync();
        return request;
    }

    public async Task UpdateStatusAsync(Guid id, MaintenanceStatus status, string? staffNote)
    {
        var request = await _db.MaintenanceRequests.FindAsync(id)
            ?? throw new InvalidOperationException("Request not found.");

        request.Status    = status;
        request.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(staffNote))
            request.StaffNote = staffNote.Trim();

        if (status == MaintenanceStatus.Resolved || status == MaintenanceStatus.Closed)
            request.ResolvedAt ??= DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }
}
