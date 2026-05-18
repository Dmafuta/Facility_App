using FacilityApp.Data.Models;

namespace FacilityApp.Services;

public interface IAuditService
{
    Task LogAsync(string action, string entityType, string? entityId = null,
        string? details = null, string? userId = null, string? userName = null);

    Task<(List<AuditLog> Items, int Total)> GetLogsAsync(
        string? search = null, int page = 1, int pageSize = 50);
}
