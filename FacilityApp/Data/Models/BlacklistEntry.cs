namespace FacilityApp.Data.Models;

public enum BlacklistType { Blacklisted, Watchlisted }

public class BlacklistEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Reason { get; set; } = string.Empty;
    public BlacklistType EntryType { get; set; } = BlacklistType.Blacklisted;
    public string? AddedByUserId { get; set; }
    public string AddedByName { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}
