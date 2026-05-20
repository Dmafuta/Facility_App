namespace FacilityApp.Data.Models;

public class Vehicle
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    public string OwnerId { get; set; } = string.Empty;
    public ApplicationUser Owner { get; set; } = null!;

    public string PlateNumber { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Colour { get; set; } = string.Empty;
    public VehicleType Type { get; set; } = VehicleType.Car;
    public string? Notes { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public VehicleTag? Tag { get; set; }
}

public enum VehicleType { Car, Motorcycle, Truck, Van, Other }
