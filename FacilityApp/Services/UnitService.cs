using FacilityApp.Data;
using FacilityApp.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FacilityApp.Services;

public class UnitService : IUnitService
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TenantContext _tenantCtx;

    public UnitService(AppDbContext context, UserManager<ApplicationUser> userManager, TenantContext tenantCtx)
    {
        _context    = context;
        _userManager = userManager;
        _tenantCtx  = tenantCtx;
    }

    public async Task<List<UnitDetails>> GetAllAsync()
    {
        var units = await _context.Units
            .Include(u => u.UserUnits)
                .ThenInclude(uu => uu.User)
            .OrderBy(u => u.Block)
            .ThenBy(u => u.UnitNumber)
            .ToListAsync();

        return units.Select(u => new UnitDetails(
            u,
            u.UserUnits.FirstOrDefault(uu => uu.LinkType == UnitLinkType.Owner)?.User,
            u.UserUnits.FirstOrDefault(uu => uu.LinkType == UnitLinkType.Occupant)?.User
        )).ToList();
    }

    public async Task<Unit> CreateAsync(string unitNumber, string? block, string? floor, string? description)
    {
        var unit = new Unit
        {
            TenantId    = _tenantCtx.TenantId,
            UnitNumber  = unitNumber.Trim(),
            Block       = block?.Trim(),
            Floor       = floor?.Trim(),
            Description = description?.Trim()
        };
        _context.Units.Add(unit);
        await _context.SaveChangesAsync();
        return unit;
    }

    public async Task UpdateAsync(Guid unitId, string unitNumber, string? block, string? floor, string? description)
    {
        var unit = await _context.Units.FindAsync(unitId)
            ?? throw new InvalidOperationException("Unit not found.");
        unit.UnitNumber  = unitNumber.Trim();
        unit.Block       = block?.Trim();
        unit.Floor       = floor?.Trim();
        unit.Description = description?.Trim();
        await _context.SaveChangesAsync();
    }

    public async Task AssignOwnerAsync(Guid unitId, string userId)
    {
        // Remove existing owner link for this unit
        var existing = await _context.UserUnits
            .FirstOrDefaultAsync(uu => uu.UnitId == unitId && uu.LinkType == UnitLinkType.Owner);
        if (existing is not null)
            _context.UserUnits.Remove(existing);

        _context.UserUnits.Add(new UserUnit
        {
            TenantId = _tenantCtx.TenantId,
            UnitId   = unitId,
            UserId   = userId,
            LinkType = UnitLinkType.Owner
        });
        await _context.SaveChangesAsync();
    }

    public async Task RemoveOwnerAsync(Guid unitId)
    {
        var link = await _context.UserUnits
            .FirstOrDefaultAsync(uu => uu.UnitId == unitId && uu.LinkType == UnitLinkType.Owner);
        if (link is null) return;
        _context.UserUnits.Remove(link);
        await _context.SaveChangesAsync();
    }

    public async Task AssignOccupantAsync(Guid unitId, string userId)
    {
        // Remove current occupant link (and role if they have no other occupant links)
        var existing = await _context.UserUnits
            .FirstOrDefaultAsync(uu => uu.UnitId == unitId && uu.LinkType == UnitLinkType.Occupant);

        if (existing is not null)
        {
            var oldUser = await _userManager.FindByIdAsync(existing.UserId);
            _context.UserUnits.Remove(existing);
            await _context.SaveChangesAsync();

            if (oldUser is not null)
                await RevokeOccupantRoleIfUnlinkedAsync(oldUser);
        }

        _context.UserUnits.Add(new UserUnit
        {
            TenantId = _tenantCtx.TenantId,
            UnitId   = unitId,
            UserId   = userId,
            LinkType = UnitLinkType.Occupant
        });

        var unit = await _context.Units.FindAsync(unitId);
        if (unit is not null) unit.IsOccupied = true;

        await _context.SaveChangesAsync();

        // Grant Occupant role to new occupant
        var newUser = await _userManager.FindByIdAsync(userId);
        if (newUser is not null && !await _userManager.IsInRoleAsync(newUser, Program.RoleOccupant))
            await _userManager.AddToRoleAsync(newUser, Program.RoleOccupant);
    }

    public async Task RemoveOccupantAsync(Guid unitId)
    {
        var link = await _context.UserUnits
            .FirstOrDefaultAsync(uu => uu.UnitId == unitId && uu.LinkType == UnitLinkType.Occupant);
        if (link is null) return;

        var user = await _userManager.FindByIdAsync(link.UserId);
        _context.UserUnits.Remove(link);

        var unit = await _context.Units.FindAsync(unitId);
        if (unit is not null) unit.IsOccupied = false;

        await _context.SaveChangesAsync();

        if (user is not null)
            await RevokeOccupantRoleIfUnlinkedAsync(user);
    }

    public async Task DeleteAsync(Guid unitId)
    {
        var unit = await _context.Units
            .Include(u => u.UserUnits)
            .FirstOrDefaultAsync(u => u.Id == unitId)
            ?? throw new InvalidOperationException("Unit not found.");

        if (unit.UserUnits.Any(uu => uu.LinkType == UnitLinkType.Occupant))
            throw new InvalidOperationException("Cannot delete a unit with an active occupant. Remove the occupant first.");

        _context.UserUnits.RemoveRange(unit.UserUnits);
        _context.Units.Remove(unit);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ApplicationUser>> GetAssignableUsersAsync(UnitLinkType linkType)
    {
        // TenantContext is set, so query filter applies automatically
        var users = _context.Users.AsQueryable();

        return linkType == UnitLinkType.Owner
            ? await users.Where(u => u.UserType == UserType.HomeOwner)
                         .OrderBy(u => u.FullName)
                         .ToListAsync()
            : await users.Where(u => u.UserType == UserType.Resident || u.UserType == UserType.HomeOwner)
                         .OrderBy(u => u.UserType)
                         .ThenBy(u => u.FullName)
                         .ToListAsync();
    }

    public async Task<List<(Unit Unit, UnitLinkType LinkType)>> GetForUserAsync(string userId)
    {
        var links = await _context.UserUnits
            .Include(uu => uu.Unit)
            .Where(uu => uu.UserId == userId)
            .OrderBy(uu => uu.Unit.Block)
            .ThenBy(uu => uu.Unit.UnitNumber)
            .ToListAsync();

        return links.Select(uu => (uu.Unit, uu.LinkType)).ToList();
    }

    // Removes Occupant role only if the user has no remaining Occupant UserUnit links
    private async Task RevokeOccupantRoleIfUnlinkedAsync(ApplicationUser user)
    {
        var stillOccupant = await _context.UserUnits
            .AnyAsync(uu => uu.UserId == user.Id && uu.LinkType == UnitLinkType.Occupant);

        if (!stillOccupant && await _userManager.IsInRoleAsync(user, Program.RoleOccupant))
            await _userManager.RemoveFromRoleAsync(user, Program.RoleOccupant);
    }
}
