using FacilityApp.Data;
using FacilityApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FacilityApp.Services;

public class TenantService : ITenantService
{
    private readonly AppDbContext _db;

    public TenantService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Tenant?> ResolveBySlugAsync(string slug)
    {
        return await _db.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Slug == slug.ToLower() && t.IsActive);
    }

    public async Task<Tenant?> ResolveByDomainAsync(string host)
    {
        return await _db.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.CustomDomain == host.ToLower() && t.IsActive);
    }
}
