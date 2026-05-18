using FacilityApp.Data.Models;

namespace FacilityApp.Services;

public record UnitDetails(
    Unit Unit,
    ApplicationUser? Owner,
    ApplicationUser? Occupant
);

public interface IUnitService
{
    Task<List<UnitDetails>> GetAllAsync();
    Task<Unit> CreateAsync(string unitNumber, string? block, string? floor, string? description);
    Task UpdateAsync(Guid unitId, string unitNumber, string? block, string? floor, string? description);
    Task AssignOwnerAsync(Guid unitId, string userId);
    Task RemoveOwnerAsync(Guid unitId);
    Task AssignOccupantAsync(Guid unitId, string userId);
    Task RemoveOccupantAsync(Guid unitId);

    Task DeleteAsync(Guid unitId);

    /// <summary>Returns users eligible for assignment to a unit.</summary>
    /// <param name="linkType">Owner → HomeOwners only. Occupant → Residents + HomeOwners.</param>
    Task<List<ApplicationUser>> GetAssignableUsersAsync(UnitLinkType linkType);

    /// <summary>Returns all units the given user is linked to (as owner or occupant).</summary>
    Task<List<(Unit Unit, UnitLinkType LinkType)>> GetForUserAsync(string userId);
}
