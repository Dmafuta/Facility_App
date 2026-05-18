using FacilityApp.Data;
using FacilityApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FacilityApp.Services;

public class SettingsService : ISettingsService
{
    private readonly AppDbContext _context;
    private readonly TenantContext _tenantCtx;

    public SettingsService(AppDbContext context, TenantContext tenantCtx)
    {
        _context   = context;
        _tenantCtx = tenantCtx;
    }

    public async Task<Tenant?> GetAsync()
        => await _context.Tenants.FindAsync(_tenantCtx.TenantId);

    public async Task UpdateAsync(string name, string? contactEmail, string? contactPhone, string? address, string? website, string? customDomain)
    {
        var tenant = await _context.Tenants.FindAsync(_tenantCtx.TenantId)
            ?? throw new InvalidOperationException("Tenant not found.");
        tenant.Name         = name.Trim();
        tenant.ContactEmail = contactEmail?.Trim();
        tenant.ContactPhone = contactPhone?.Trim();
        tenant.Address      = address?.Trim();
        tenant.Website      = website?.Trim();
        tenant.CustomDomain = string.IsNullOrWhiteSpace(customDomain) ? null : customDomain.Trim().ToLower();
        await _context.SaveChangesAsync();

        _tenantCtx.TenantName = tenant.Name;
    }

    public async Task UpdateBrandingAsync(string? logoUrl, string? primaryColour)
    {
        var tenant = await _context.Tenants.FindAsync(_tenantCtx.TenantId)
            ?? throw new InvalidOperationException("Tenant not found.");
        tenant.LogoUrl       = logoUrl?.Trim();
        tenant.PrimaryColour = primaryColour?.Trim();
        await _context.SaveChangesAsync();

        _tenantCtx.LogoUrl       = tenant.LogoUrl;
        _tenantCtx.PrimaryColour = tenant.PrimaryColour;
    }
}
