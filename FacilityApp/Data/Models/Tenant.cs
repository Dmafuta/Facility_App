namespace FacilityApp.Data.Models;

public class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Settings
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Address { get; set; }
    public string? Website { get; set; }

    // Branding
    public string? LogoUrl { get; set; }
    public string? PrimaryColour { get; set; }  // hex e.g. #1b6ec2

    // Custom domain (e.g. greatwallgardens.estate) — enables slug-free URLs
    public string? CustomDomain { get; set; }
}
