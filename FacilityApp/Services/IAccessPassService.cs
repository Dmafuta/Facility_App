using FacilityApp.Data.Models;

namespace FacilityApp.Services;

public interface IAccessPassService
{
    Task<AccessPass> GenerateAsync(Guid visitId, PassType passType,
        string? vehicleRegistration, string? parkingBay, DateTime? validUntil);
    Task RevokeAsync(Guid passId, string? reason);
    Task<(List<AccessPass> Items, int Total)> GetPassesAsync(
        string? status, string? search, int page = 1, int pageSize = 25);
    Task<AccessPass?> GetForVisitAsync(Guid visitId);
}
