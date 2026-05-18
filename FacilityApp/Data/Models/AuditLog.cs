namespace FacilityApp.Data.Models;

public class AuditLog
{
    public long Id { get; set; }
    public Guid TenantId { get; set; }
    public string? UserId { get; set; }
    public string UserName { get; set; } = "System";
    public string Action { get; set; } = "";
    public string EntityType { get; set; } = "";
    public string? EntityId { get; set; }
    public string? Details { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
