namespace FacilityApp.Data.Models;

public class VehicleTag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    public Guid VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    public string TagNumber { get; set; } = string.Empty;   // e.g. TAG-0042
    public TagStatus Status { get; set; } = TagStatus.Active;

    public string IssuedById { get; set; } = string.Empty;
    public ApplicationUser IssuedBy { get; set; } = null!;

    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public string? Notes { get; set; }
}

public enum TagStatus { Active, Suspended, Revoked, Expired }
