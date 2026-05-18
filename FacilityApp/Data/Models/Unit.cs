namespace FacilityApp.Data.Models;

public class Unit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public string UnitNumber { get; set; } = string.Empty;
    public string? Block { get; set; }
    public string? Floor { get; set; }
    public string? Description { get; set; }
    public bool IsOccupied { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserUnit> UserUnits { get; set; } = [];
}
