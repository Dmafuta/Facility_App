using Microsoft.AspNetCore.Identity;

namespace FacilityApp.Data.Models;

public class ApplicationUser : IdentityUser
{
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public string FullName { get; set; } = string.Empty;
    public UserType UserType { get; set; } = UserType.Staff;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<UserUnit> UserUnits { get; set; } = [];
}
