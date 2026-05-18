using FacilityApp.Data;
using FacilityApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FacilityApp.Services;

public class FacilityService : IFacilityService
{
    private readonly AppDbContext _context;
    private readonly TenantContext _tenantCtx;

    public FacilityService(AppDbContext context, TenantContext tenantCtx)
    {
        _context   = context;
        _tenantCtx = tenantCtx;
    }

    public async Task<List<Facility>> GetAllAsync()
        => await _context.Facilities
            .OrderBy(f => f.Name)
            .ToListAsync();

    public async Task<Facility> CreateAsync(string name, string? location, int? capacity)
    {
        var facility = new Facility
        {
            TenantId = _tenantCtx.TenantId,
            Name     = name.Trim(),
            Location = location?.Trim(),
            Capacity = capacity,
            IsActive = true
        };
        _context.Facilities.Add(facility);
        await _context.SaveChangesAsync();
        return facility;
    }

    public async Task UpdateAsync(Guid id, string name, string? location, int? capacity)
    {
        var f = await _context.Facilities.FindAsync(id)
            ?? throw new InvalidOperationException("Facility not found.");
        f.Name     = name.Trim();
        f.Location = location?.Trim();
        f.Capacity = capacity;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var f = await _context.Facilities.FindAsync(id)
            ?? throw new InvalidOperationException("Facility not found.");
        _context.Facilities.Remove(f);
        await _context.SaveChangesAsync();
    }

    public async Task ToggleActiveAsync(Guid id)
    {
        var f = await _context.Facilities.FindAsync(id)
            ?? throw new InvalidOperationException("Facility not found.");
        f.IsActive = !f.IsActive;
        await _context.SaveChangesAsync();
    }
}
