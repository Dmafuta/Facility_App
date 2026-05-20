namespace FacilityApp.Data.Models;

public class ParkingRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    // Resident vehicle link (nullable — visitors have no registered vehicle)
    public Guid? VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    public Guid? VehicleTagId { get; set; }
    public VehicleTag? VehicleTag { get; set; }

    // Always captured
    public string PlateNumber { get; set; } = string.Empty;
    public ParkingRecordType Type { get; set; } = ParkingRecordType.Visitor;

    // Optional link to a visitor visit
    public Guid? VisitId { get; set; }
    public Visit? Visit { get; set; }

    // Gate info
    public Guid? EntryEntranceId { get; set; }
    public Entrance? EntryEntrance { get; set; }

    public Guid? ExitEntranceId { get; set; }
    public Entrance? ExitEntrance { get; set; }

    // Who logged it
    public string LoggedById { get; set; } = string.Empty;
    public ApplicationUser LoggedBy { get; set; } = null!;

    public DateTime EnteredAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExitedAt { get; set; }
    public string? Notes { get; set; }
}

public enum ParkingRecordType { Resident, Visitor, Unknown }
