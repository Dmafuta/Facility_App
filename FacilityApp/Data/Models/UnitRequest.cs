namespace FacilityApp.Data.Models;

public class UnitRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    public string ResidentId { get; set; } = string.Empty;
    public ApplicationUser Resident { get; set; } = null!;

    public Guid UnitId { get; set; }
    public Unit Unit { get; set; } = null!;

    public UnitRequestStatus Status { get; set; } = UnitRequestStatus.Pending;

    /// <summary>Optional note from the resident (e.g. move-in date, lease reference).</summary>
    public string? Note { get; set; }

    /// <summary>Admin/manager rejection reason.</summary>
    public string? ReviewNote { get; set; }

    public string? ReviewedById { get; set; }
    public ApplicationUser? ReviewedBy { get; set; }

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }
}

public enum UnitRequestStatus
{
    Pending,
    Approved,
    Rejected
}
