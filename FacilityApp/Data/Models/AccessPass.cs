namespace FacilityApp.Data.Models;

public enum PassType { Gate, Parking, Both }

public class AccessPass
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid VisitId { get; set; }
    public Visit Visit { get; set; } = null!;
    public PassType PassType { get; set; } = PassType.Gate;
    public string PassNumber { get; set; } = string.Empty;
    public string? VehicleRegistration { get; set; }
    public string? ParkingBay { get; set; }
    public DateTime ValidFrom { get; set; } = DateTime.UtcNow;
    public DateTime? ValidUntil { get; set; }
    public Guid? EntranceId { get; set; }
    public Entrance? Entrance { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevokedReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
