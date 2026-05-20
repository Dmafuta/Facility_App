using FacilityApp.Data.Models;

namespace FacilityApp.Services;

public interface IEntranceService
{
    Task<List<Entrance>> GetActiveAsync();
    Task<List<Entrance>> GetAllAsync();
    Task<Entrance?> GetByIdAsync(Guid id);
    Task<Entrance> CreateAsync(string name, string? description);
    Task UpdateAsync(Guid id, string name, string? description);
    Task ToggleActiveAsync(Guid id);
    Task DeleteAsync(Guid id);
    Task SetCurrentEntranceAsync(string userId, Guid entranceId);
}
