using Microsoft.AspNetCore.SignalR;

namespace FacilityApp.Hubs;

/// <summary>
/// Clients join a group named after their tenantSlug.
/// Server broadcasts "VisitorCheckedIn" to that group on every check-in.
/// </summary>
public class NotificationHub : Hub
{
    public async Task JoinTenant(string tenantSlug)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, tenantSlug);
    }

    public async Task LeaveTenant(string tenantSlug)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, tenantSlug);
    }
}
