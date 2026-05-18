using FacilityApp.Data.Models;

namespace FacilityApp.Services;

public interface IFacilityService
{
    Task<List<Facility>> GetAllAsync();
    Task<Facility> CreateAsync(string name, string? location, int? capacity);
    Task UpdateAsync(Guid id, string name, string? location, int? capacity);
    Task DeleteAsync(Guid id);
    Task ToggleActiveAsync(Guid id);
}
