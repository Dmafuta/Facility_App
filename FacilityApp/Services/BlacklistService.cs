using FacilityApp.Data;
using FacilityApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FacilityApp.Services;

public class BlacklistService : IBlacklistService
{
    private readonly AppDbContext _db;
    private readonly TenantContext _tenantCtx;
    private readonly IAuditService _audit;

    public BlacklistService(AppDbContext db, TenantContext tenantCtx, IAuditService audit)
    {
        _db        = db;
        _tenantCtx = tenantCtx;
        _audit     = audit;
    }

    public async Task<BlacklistEntry> AddAsync(
        string fullName, string? email, string? phone,
        string reason, BlacklistType entryType,
        DateTime? expiresAt, string? notes,
        string addedByUserId, string addedByName)
    {
        var entry = new BlacklistEntry
        {
            TenantId       = _tenantCtx.TenantId,
            FullName       = fullName.Trim(),
            Email          = email?.Trim().ToLower(),
            Phone          = phone?.Trim(),
            Reason         = reason.Trim(),
            EntryType      = entryType,
            ExpiresAt      = expiresAt,
            Notes          = notes?.Trim(),
            AddedByUserId  = addedByUserId,
            AddedByName    = addedByName,
            IsActive       = true
        };
        _db.BlacklistEntries.Add(entry);
        await _db.SaveChangesAsync();

        await _audit.LogAsync(
            entryType == BlacklistType.Blacklisted ? "Blacklist" : "Watchlist",
            "BlacklistEntry", entry.Id.ToString(),
            $"{fullName} added — reason: {reason}",
            addedByUserId, addedByName);

        return entry;
    }

    public async Task RemoveAsync(Guid id)
    {
        var entry = await _db.BlacklistEntries.FindAsync(id)
            ?? throw new InvalidOperationException("Entry not found.");
        entry.IsActive = false;
        await _db.SaveChangesAsync();

        await _audit.LogAsync("RemoveFromList", "BlacklistEntry", id.ToString(),
            $"{entry.FullName} removed from {entry.EntryType}");
    }

    public async Task<(List<BlacklistEntry> Items, int Total)> GetEntriesAsync(
        string? search, string? type, int page = 1, int pageSize = 25)
    {
        var q = _db.BlacklistEntries.Where(e => e.IsActive).AsQueryable();

        if (type == "blacklisted")
            q = q.Where(e => e.EntryType == BlacklistType.Blacklisted);
        else if (type == "watchlisted")
            q = q.Where(e => e.EntryType == BlacklistType.Watchlisted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            q = q.Where(e =>
                e.FullName.ToLower().Contains(s) ||
                (e.Email != null && e.Email.Contains(s)) ||
                (e.Phone != null && e.Phone.Contains(s)) ||
                e.Reason.ToLower().Contains(s));
        }

        var total = await q.CountAsync();
        var items = await q
            .OrderByDescending(e => e.AddedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<BlacklistEntry?> CheckAsync(string? email, string? phone)
    {
        var now = DateTime.UtcNow;
        var emailNorm = email?.Trim().ToLower();

        return await _db.BlacklistEntries
            .Where(e => e.IsActive &&
                        (e.ExpiresAt == null || e.ExpiresAt > now) &&
                        ((emailNorm != null && e.Email == emailNorm) ||
                         (phone != null && e.Phone == phone.Trim())))
            .OrderBy(e => e.EntryType) // Blacklisted=0 sorts first, so blocked visitors surface before watchlisted
            .FirstOrDefaultAsync();
    }
}
