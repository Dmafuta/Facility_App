using FacilityApp.Data.Models;

namespace FacilityApp.Services;

public interface IMaintenanceService
{
    Task<List<MaintenanceRequest>> GetForResidentAsync(string residentId);
    Task<List<MaintenanceRequest>> GetAllAsync(MaintenanceStatus? status = null);
    Task<int> GetOpenCountAsync();
    Task<MaintenanceRequest> SubmitAsync(string residentId, Guid? unitId, string title, string description, MaintenanceCategory category, MaintenancePriority priority);
    Task UpdateStatusAsync(Guid id, MaintenanceStatus status, string? staffNote);
}
