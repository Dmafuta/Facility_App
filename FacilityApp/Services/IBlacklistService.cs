using FacilityApp.Data.Models;

namespace FacilityApp.Services;

public interface IBlacklistService
{
    Task<BlacklistEntry> AddAsync(string fullName, string? email, string? phone,
        string reason, BlacklistType entryType, DateTime? expiresAt, string? notes,
        string addedByUserId, string addedByName);
    Task RemoveAsync(Guid id);
    Task<(List<BlacklistEntry> Items, int Total)> GetEntriesAsync(
        string? search, string? type, int page = 1, int pageSize = 25);
    Task<BlacklistEntry?> CheckAsync(string? email, string? phone);
}
