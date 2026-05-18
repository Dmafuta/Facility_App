using FacilityApp.Data;
using FacilityApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FacilityApp.Services;

public class UnitRequestService : IUnitRequestService
{
    private readonly AppDbContext _db;
    private readonly IUnitService _unitSvc;
    private readonly TenantContext _tenantCtx;

    public UnitRequestService(AppDbContext db, IUnitService unitSvc, TenantContext tenantCtx)
    {
        _db        = db;
        _unitSvc   = unitSvc;
        _tenantCtx = tenantCtx;
    }

    public async Task<UnitRequest?> GetForResidentAsync(string residentId)
    {
        return await _db.UnitRequests
            .Include(r => r.Unit)
            .Include(r => r.ReviewedBy)
            .Where(r => r.ResidentId == residentId)
            .OrderByDescending(r => r.RequestedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<List<UnitRequest>> GetPendingAsync()
    {
        return await _db.UnitRequests
            .Include(r => r.Unit)
            .Include(r => r.Resident)
            .Where(r => r.Status == UnitRequestStatus.Pending)
            .OrderBy(r => r.RequestedAt)
            .ToListAsync();
    }

    public async Task<int> GetPendingCountAsync()
    {
        return await _db.UnitRequests
            .CountAsync(r => r.Status == UnitRequestStatus.Pending);
    }

    public async Task<UnitRequest> SubmitAsync(string residentId, Guid unitId, string? note)
    {
        var hasPending = await _db.UnitRequests
            .AnyAsync(r => r.ResidentId == residentId && r.Status == UnitRequestStatus.Pending);

        if (hasPending)
            throw new InvalidOperationException("You already have a pending unit request.");

        var request = new UnitRequest
        {
            TenantId   = _tenantCtx.TenantId,
            ResidentId = residentId,
            UnitId     = unitId,
            Note       = note?.Trim()
        };

        _db.UnitRequests.Add(request);
        await _db.SaveChangesAsync();
        return request;
    }

    public async Task ApproveAsync(Guid requestId, string reviewerId)
    {
        var request = await _db.UnitRequests.FindAsync(requestId)
            ?? throw new InvalidOperationException("Request not found.");

        if (request.Status != UnitRequestStatus.Pending)
            throw new InvalidOperationException("Request is no longer pending.");

        // Assign the unit occupant (this also grants the Occupant role)
        await _unitSvc.AssignOccupantAsync(request.UnitId, request.ResidentId);

        request.Status      = UnitRequestStatus.Approved;
        request.ReviewedById = reviewerId;
        request.ReviewedAt  = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }

    public async Task RejectAsync(Guid requestId, string reviewerId, string? reviewNote)
    {
        var request = await _db.UnitRequests.FindAsync(requestId)
            ?? throw new InvalidOperationException("Request not found.");

        if (request.Status != UnitRequestStatus.Pending)
            throw new InvalidOperationException("Request is no longer pending.");

        request.Status       = UnitRequestStatus.Rejected;
        request.ReviewedById  = reviewerId;
        request.ReviewNote   = reviewNote?.Trim();
        request.ReviewedAt   = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }
}
