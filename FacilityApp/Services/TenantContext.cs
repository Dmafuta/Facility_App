using FacilityApp.Data.Models;

namespace FacilityApp.Services;

public class TenantContext
{
    public Guid TenantId { get; set; } = Guid.Empty;
    public string TenantSlug { get; set; } = string.Empty;
    public string TenantName { get; set; } = string.Empty;
    public bool IsResolved { get; set; }
    public string? PrimaryColour { get; set; }
    public string? LogoUrl { get; set; }
    public TenantPlan Plan { get; set; } = TenantPlan.Starter;

    /// <summary>True when the tenant was resolved from a custom hostname (e.g. greatwallgardens.estate).</summary>
    public bool IsCustomDomain { get; set; }

    /// <summary>
    /// URL prefix for generating links.
    /// Empty string on a custom domain, "/{slug}" on a shared domain.
    /// Use as: href="@(TenantCtx.RouteBase)/dashboard"
    /// </summary>
    public string RouteBase => IsCustomDomain ? "" : (string.IsNullOrEmpty(TenantSlug) ? "" : $"/{TenantSlug}");
}
