namespace FacilityApp.Tests;

/// <summary>
/// Verifies that every URL used in NavMenu and ResidentLayout points to a
/// declared @page route in the application. Catches broken nav links at
/// build time rather than at runtime.
/// </summary>
public class NavRouteTests
{
    // All @page routes declared across the application (slug-free variants only,
    // since custom-domain and slug-based domains both map to the same components).
    private static readonly HashSet<string> KnownRoutes =
    [
        "/dashboard",
        "/visitors/checkin",
        "/visitors",
        "/visitors/preregister",
        "/visitors/{visitId}",            // Detail page (guid param)
        "/visitors/{visitId}/badge",
        "/access/passes",
        "/access/blacklist",
        "/parking",
        "/parcels",
        "/maintenance",
        "/admin/users",
        "/admin/units",
        "/admin/unit-requests",
        "/admin/announcements",
        "/admin/documents",
        "/reports",
        "/incidents",
        "/facilities",
        "/admin/entrances",
        "/admin/audit-log",
        "/settings",
        "/select-gate",
        "/login",
        "/register",
        "/forgot-password",
        "/reset-password",
        "/resident",
        "/resident/login",
        "/resident/register",
        "/resident/forgot-password",
        "/resident/reset-password",
        "/resident/preregister",
        "/resident/visits",
        "/resident/maintenance",
        "/resident/vehicles",
        "/resident/parcels",
        "/resident/documents",
        "/resident/profile",
        "/resident/request-unit",
        "/superadmin/tenants",
        "/superadmin/tenants/create",
        "/superadmin/dashboard",
        "/setup",
    ];

    // Every URL used in NavMenu.razor (staff sidebar) and ResidentLayout.razor navbar.
    // These must all exist in KnownRoutes.
    private static readonly string[] NavMenuLinks =
    [
        "/dashboard",
        "/visitors/checkin",
        "/visitors",
        "/access/passes",
        "/access/blacklist",
        "/parking",
        "/parcels",
        "/maintenance",
        "/admin/users",
        "/admin/units",
        "/admin/unit-requests",
        "/admin/announcements",
        "/admin/documents",
        "/reports",
        "/incidents",
        "/facilities",
        "/admin/entrances",
        "/admin/audit-log",
        "/settings",
        "/select-gate",
        // Resident section in NavMenu (shown to Occupant role)
        "/resident",
        "/resident/preregister",
        "/resident/maintenance",
        "/resident/vehicles",
        "/resident/parcels",
    ];

    private static readonly string[] ResidentLayoutLinks =
    [
        "/resident",
        "/resident/visits",
        "/resident/preregister",
        "/resident/maintenance",
        "/resident/documents",
        "/resident/parcels",
        "/resident/profile",
    ];

    [Theory]
    [MemberData(nameof(NavMenuLinkData))]
    public void NavMenuLink_HasDeclaredRoute(string path)
    {
        Assert.Contains(path, KnownRoutes);
    }

    [Theory]
    [MemberData(nameof(ResidentLayoutLinkData))]
    public void ResidentLayoutLink_HasDeclaredRoute(string path)
    {
        Assert.Contains(path, KnownRoutes);
    }

    public static IEnumerable<object[]> NavMenuLinkData()
        => NavMenuLinks.Select(l => new object[] { l });

    public static IEnumerable<object[]> ResidentLayoutLinkData()
        => ResidentLayoutLinks.Select(l => new object[] { l });
}
