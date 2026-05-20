namespace FacilityApp.Data.Models;

public class Parcel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    public Guid? UnitId { get; set; }
    public Unit? Unit { get; set; }

    public string RecipientName { get; set; } = string.Empty;
    public string? CourierName { get; set; }
    public string Description { get; set; } = string.Empty;

    public string ReceivedById { get; set; } = string.Empty;
    public ApplicationUser ReceivedBy { get; set; } = null!;
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

    public ParcelStatus Status { get; set; } = ParcelStatus.Pending;
    public DateTime? CollectedAt { get; set; }
    public string? CollectedByName { get; set; }

    public string? Notes { get; set; }
}

public enum ParcelStatus { Pending, Collected, Returned }
