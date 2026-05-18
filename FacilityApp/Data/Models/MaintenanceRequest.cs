namespace FacilityApp.Data.Models;

public class MaintenanceRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    public string ResidentId { get; set; } = string.Empty;
    public ApplicationUser Resident { get; set; } = null!;

    public Guid? UnitId { get; set; }
    public Unit? Unit { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public MaintenanceCategory Category { get; set; }
    public MaintenancePriority Priority { get; set; }
    public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Open;

    public string? StaffNote { get; set; }
    public string? AssignedToId { get; set; }
    public ApplicationUser? AssignedTo { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public enum MaintenanceCategory
{
    Plumbing,
    Electrical,
    HVAC,
    Structural,
    Appliance,
    Pest,
    Cleaning,
    Other
}

public enum MaintenancePriority { Low, Medium, High, Urgent }

public enum MaintenanceStatus { Open, InProgress, Resolved, Closed }
