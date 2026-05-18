namespace FacilityApp.Data.Models;

public enum VisitStatus
{
    Scheduled,
    CheckedIn,
    CheckedOut,
    Cancelled,
    NoShow
}

public class Visit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public Guid VisitorId { get; set; }
    public Visitor Visitor { get; set; } = null!;
    public string? HostUserId { get; set; }
    public ApplicationUser? Host { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public DateTime? CheckedInAt { get; set; }
    public DateTime? CheckedOutAt { get; set; }
    public VisitStatus Status { get; set; } = VisitStatus.Scheduled;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
