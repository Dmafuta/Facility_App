namespace FacilityApp.Controllers;

// ── Auth ────────────────────────────────────────────────────────────────────

public record LoginRequest(string Slug, string Email, string Password);

public record LoginResponse(
    string   AccessToken,
    string   RefreshToken,
    DateTime ExpiresAt,
    UserDto  User);

public record UserDto(
    string   Id,
    string   Name,
    string   Email,
    string[] Roles,
    string   TenantSlug,
    string   TenantName,
    string   UserType);

public record RefreshRequest(string RefreshToken);

public record RefreshResponse(string AccessToken, DateTime ExpiresAt);

public record LogoutRequest(string RefreshToken);

public record ForgotPasswordRequest(string Slug, string Email);

public record ResetPasswordRequest(
    string Slug,
    string Email,
    string Token,
    string NewPassword);

// ── SuperAdmin ───────────────────────────────────────────────────────────────

public record TenantDto(
    Guid     Id,
    string   Name,
    string   Slug,
    bool     IsActive,
    int      Plan,
    string?  CustomDomain,
    string?  ContactEmail,
    string?  ContactPhone,
    string?  Address,
    string?  Website,
    string?  PrimaryColour,
    string?  LogoUrl,
    DateTime CreatedAt);

public record CreateTenantRequest(string Name, string Slug, string ContactEmail);

public record UpdatePlanRequest(int Plan);

// ── Dashboard ────────────────────────────────────────────────────────────────

public record DashboardResponse(
    int     TodayVisitors,
    int     ActiveVisitors,
    int     PendingParcels,
    int     OpenMaintenance,
    int     TotalUnits,
    int     OccupiedUnits);
