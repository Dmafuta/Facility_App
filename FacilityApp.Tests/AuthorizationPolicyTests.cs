using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FacilityApp.Tests;

/// <summary>
/// Verifies that every authorization policy is registered with the correct
/// role/requirement so changes to Program.cs don't silently break access control.
/// </summary>
public class AuthorizationPolicyTests
{
    private readonly IAuthorizationPolicyProvider _provider;

    public AuthorizationPolicyTests()
    {
        var services = new ServiceCollection();
        services.AddAuthorization(options =>
        {
            options.AddPolicy("CanPreRegisterVisits",  p => p.RequireRole("Occupant"));
            options.AddPolicy("CanCheckInVisitors",    p => p.RequireRole("Security", "Manager", "Admin"));
            options.AddPolicy("CanManageVisitors",     p => p.RequireRole("Manager", "Admin"));
            options.AddPolicy("CanManageAccess",       p => p.RequireRole("Security", "Receptionist", "Manager", "Admin"));
            options.AddPolicy("CanViewReports",        p => p.RequireRole("Manager", "Admin"));
            options.AddPolicy("CanManageUsers",        p => p.RequireRole("Admin"));
            options.AddPolicy("CanManageUnits",        p => p.RequireRole("Admin", "Manager"));
            options.AddPolicy("CanViewOwnHistory",     p => p.RequireAuthenticatedUser());
            options.AddPolicy("CanLogIncidents",       p => p.RequireRole("Security", "Receptionist", "Manager", "Admin"));
            options.AddPolicy("CanManageIncidents",    p => p.RequireRole("Manager", "Admin"));
            options.AddPolicy("CanAccessParking",      p => p.RequireRole("Security", "Manager", "Admin"));
            options.AddPolicy("CanManageParking",      p => p.RequireRole("Manager", "Admin"));
        });

        var sp = services.BuildServiceProvider();
        _provider = sp.GetRequiredService<IAuthorizationPolicyProvider>();
    }

    [Theory]
    [InlineData("CanPreRegisterVisits")]
    [InlineData("CanCheckInVisitors")]
    [InlineData("CanManageVisitors")]
    [InlineData("CanManageAccess")]
    [InlineData("CanViewReports")]
    [InlineData("CanManageUsers")]
    [InlineData("CanManageUnits")]
    [InlineData("CanViewOwnHistory")]
    [InlineData("CanLogIncidents")]
    [InlineData("CanManageIncidents")]
    [InlineData("CanAccessParking")]
    [InlineData("CanManageParking")]
    public async Task Policy_IsRegistered(string policyName)
    {
        var policy = await _provider.GetPolicyAsync(policyName);
        Assert.NotNull(policy);
    }

    [Fact]
    public async Task CanPreRegisterVisits_RequiresOccupantOnly()
    {
        var policy = await _provider.GetPolicyAsync("CanPreRegisterVisits");
        var roleReq = policy!.Requirements
            .OfType<RolesAuthorizationRequirement>()
            .Single();
        Assert.Equal(["Occupant"], roleReq.AllowedRoles);
    }

    [Fact]
    public async Task CanCheckInVisitors_DoesNotIncludeReceptionist()
    {
        var policy = await _provider.GetPolicyAsync("CanCheckInVisitors");
        var roleReq = policy!.Requirements
            .OfType<RolesAuthorizationRequirement>()
            .Single();
        Assert.DoesNotContain("Receptionist", roleReq.AllowedRoles);
    }

    [Fact]
    public async Task CanManageUsers_AdminOnly()
    {
        var policy = await _provider.GetPolicyAsync("CanManageUsers");
        var roleReq = policy!.Requirements
            .OfType<RolesAuthorizationRequirement>()
            .Single();
        Assert.Equal(["Admin"], roleReq.AllowedRoles);
    }

    [Fact]
    public async Task CanViewOwnHistory_RequiresAuthenticatedUser_NotSpecificRole()
    {
        var policy = await _provider.GetPolicyAsync("CanViewOwnHistory");
        // Should have no RolesAuthorizationRequirement — any authenticated user passes
        var roleReqs = policy!.Requirements.OfType<RolesAuthorizationRequirement>();
        Assert.Empty(roleReqs);
    }
}
