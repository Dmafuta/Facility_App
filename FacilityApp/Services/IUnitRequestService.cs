using FacilityApp.Data.Models;

namespace FacilityApp.Services;

public interface IUnitRequestService
{
    /// <summary>Returns the resident's active request (Pending or last Rejected), or null.</summary>
    Task<UnitRequest?> GetForResidentAsync(string residentId);

    /// <summary>All pending requests for the current tenant (admin view).</summary>
    Task<List<UnitRequest>> GetPendingAsync();

    /// <summary>Count of pending requests (for nav badge).</summary>
    Task<int> GetPendingCountAsync();

    /// <summary>Submit a new unit request. Throws if the resident already has a pending request.</summary>
    Task<UnitRequest> SubmitAsync(string residentId, Guid unitId, string? note);

    /// <summary>Approve a request — assigns the unit occupant and grants the Occupant role.</summary>
    Task ApproveAsync(Guid requestId, string reviewerId);

    /// <summary>Reject a request with an optional note.</summary>
    Task RejectAsync(Guid requestId, string reviewerId, string? reviewNote);
}
