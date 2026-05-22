using FacilityApp.Data;
using FacilityApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FacilityApp.Services;

public class EntranceService : IEntranceService
{
    private readonly IDbContextFactory<AppDbContext> _factory;
    private readonly TenantContext _tenantCtx;

    public EntranceService(IDbContextFactory<AppDbContext> factory, TenantContext tenantCtx)
    {
        _factory   = factory;
        _tenantCtx = tenantCtx;
    }

    public async Task<List<Entrance>> GetActiveAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Entrances.Where(e => e.IsActive).OrderBy(e => e.Name).ToListAsync();
    }

    public async Task<List<Entrance>> GetAllAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Entrances.OrderBy(e => e.Name).ToListAsync();
    }

    public async Task<Entrance?> GetByIdAsync(Guid id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Entrances.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Entrance> CreateAsync(string name, string? description)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var entrance = new Entrance
        {
            TenantId    = _tenantCtx.TenantId,
            Name        = name.Trim(),
            Description = description?.Trim(),
            IsActive    = true
        };
        db.Entrances.Add(entrance);
        await db.SaveChangesAsync();
        return entrance;
    }

    public async Task UpdateAsync(Guid id, string name, string? description)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var entrance = await db.Entrances.FirstOrDefaultAsync(e => e.Id == id)
            ?? throw new InvalidOperationException("Entrance not found.");
        entrance.Name        = name.Trim();
        entrance.Description = description?.Trim();
        await db.SaveChangesAsync();
    }

    public async Task ToggleActiveAsync(Guid id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var entrance = await db.Entrances.FirstOrDefaultAsync(e => e.Id == id)
            ?? throw new InvalidOperationException("Entrance not found.");
        entrance.IsActive = !entrance.IsActive;
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var entrance = await db.Entrances.FirstOrDefaultAsync(e => e.Id == id)
            ?? throw new InvalidOperationException("Entrance not found.");
        db.Entrances.Remove(entrance);
        await db.SaveChangesAsync();
    }

    public async Task SetCurrentEntranceAsync(string userId, Guid entranceId)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var user = await db.Users.IgnoreQueryFilters()
                              .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new InvalidOperationException("User not found.");

        // IgnoreQueryFilters() bypasses tenant isolation, so we must verify ownership explicitly
        if (user.TenantId != _tenantCtx.TenantId)
            throw new InvalidOperationException("User does not belong to the current tenant.");

        user.CurrentEntranceId = entranceId;
        await db.SaveChangesAsync();
    }
}
