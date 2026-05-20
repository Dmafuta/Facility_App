namespace FacilityApp.Middleware;

using FacilityApp.Services;

/// <summary>
/// Resolves a tenant from the request hostname when a custom domain is configured
/// (e.g. greatwallgardens.estate). Sets TenantContext so every downstream component
/// and page can skip their own slug-based resolution.
/// </summary>
public class TenantDomainMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITenantService tenantSvc, TenantContext tenantCtx)
    {
        if (!tenantCtx.IsResolved)
        {
            var host = context.Request.Host.Host.ToLower();

            // Skip local development hosts
            bool isLocal = host == "localhost" || host == "127.0.0.1" || host.StartsWith("172.16.");
            if (!isLocal)
            {
                var tenant = await tenantSvc.ResolveByDomainAsync(host);
                if (tenant is not null)
                {
                    tenantCtx.TenantId       = tenant.Id;
                    tenantCtx.TenantSlug     = tenant.Slug;
                    tenantCtx.TenantName     = tenant.Name;
                    tenantCtx.PrimaryColour  = tenant.PrimaryColour;
                    tenantCtx.LogoUrl        = tenant.LogoUrl;
                    tenantCtx.Plan           = tenant.Plan;
                    tenantCtx.IsResolved     = true;
                    tenantCtx.IsCustomDomain = true;
                }
            }
        }

        await next(context);
    }
}
