using FacilityApp.Data;
using FacilityApp.Data.Models;
using FacilityApp.Services;
using Microsoft.EntityFrameworkCore;

namespace FacilityApp.Tests;

public class AuditServiceTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly TenantContext _tenantCtx;
    private readonly AuditService _sut;
    private readonly Guid _tenantId = Guid.NewGuid();

    public AuditServiceTests()
    {
        _tenantCtx = new TenantContext
        {
            TenantId   = _tenantId,
            TenantSlug = "test",
            TenantName = "Test Facility",
            IsResolved = true
        };

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db  = new AppDbContext(options, _tenantCtx);
        _sut = new AuditService(_db, _tenantCtx);
    }

    [Fact]
    public async Task Log_SavesEntry()
    {
        await _sut.LogAsync("CheckIn", "Visit", "visit-123", "Jane Doe checked in",
            "user-1", "jane@example.com");

        var logs = await _db.AuditLogs.IgnoreQueryFilters().ToListAsync();
        Assert.Single(logs);
        Assert.Equal("CheckIn", logs[0].Action);
        Assert.Equal("Visit", logs[0].EntityType);
        Assert.Equal("jane@example.com", logs[0].UserName);
    }

    [Fact]
    public async Task Log_DoesNotSave_WhenTenantIdIsEmpty()
    {
        _tenantCtx.TenantId = Guid.Empty;

        await _sut.LogAsync("CheckIn", "Visit");

        var count = await _db.AuditLogs.IgnoreQueryFilters().CountAsync();
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task GetLogs_ReturnsPaginatedResults()
    {
        for (var i = 0; i < 60; i++)
        {
            _db.AuditLogs.Add(new AuditLog
            {
                TenantId   = _tenantId,
                Action     = "CheckIn",
                EntityType = "Visit",
                UserName   = $"user{i}",
                CreatedAt  = DateTime.UtcNow.AddSeconds(-i)
            });
        }
        await _db.SaveChangesAsync();

        var (items, total) = await _sut.GetLogsAsync(null, page: 1, pageSize: 25);

        Assert.Equal(60, total);
        Assert.Equal(25, items.Count);
    }

    [Fact]
    public async Task GetLogs_SearchesByUserName()
    {
        _db.AuditLogs.Add(new AuditLog { TenantId = _tenantId, Action = "CheckIn", EntityType = "Visit", UserName = "alice@example.com", CreatedAt = DateTime.UtcNow });
        _db.AuditLogs.Add(new AuditLog { TenantId = _tenantId, Action = "CheckOut", EntityType = "Visit", UserName = "bob@example.com", CreatedAt = DateTime.UtcNow });
        await _db.SaveChangesAsync();

        var (items, total) = await _sut.GetLogsAsync("alice");

        Assert.Equal(1, total);
        Assert.Equal("alice@example.com", items[0].UserName);
    }

    public void Dispose() => _db.Dispose();
}
