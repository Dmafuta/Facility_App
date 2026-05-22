using FacilityApp.Data;
using FacilityApp.Data.Models;
using FacilityApp.Hubs;
using FacilityApp.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FacilityApp.Tests;

public class VisitorServiceTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly TenantContext _tenantCtx;
    private readonly Mock<IEmailService> _emailMock;
    private readonly Mock<IAuditService> _auditMock;
    private readonly Mock<IHubContext<NotificationHub>> _hubMock;
    private readonly VisitorService _sut;
    private readonly Guid _tenantId = Guid.NewGuid();

    public VisitorServiceTests()
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

        _db = new AppDbContext(options, _tenantCtx);

        _emailMock = new Mock<IEmailService>();
        _auditMock = new Mock<IAuditService>();
        _hubMock   = new Mock<IHubContext<NotificationHub>>();

        // Hub mock: allow any group/client calls without setup
        var clientsMock = new Mock<IHubClients>();
        var clientProxyMock = new Mock<IClientProxy>();
        clientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(clientProxyMock.Object);
        _hubMock.Setup(h => h.Clients).Returns(clientsMock.Object);

        var blacklistMock = new Mock<IBlacklistService>();

        _sut = new VisitorService(_db, _tenantCtx, _emailMock.Object, _auditMock.Object, _hubMock.Object, blacklistMock.Object);
    }

    // ── WalkIn ──────────────────────────────────────────────────────────

    [Fact]
    public async Task WalkIn_CreatesVisitorAndVisit()
    {
        var visit = await _sut.WalkInAsync("John Doe", "john@example.com", "555-1234",
            null, "Delivery", null);

        Assert.NotEqual(Guid.Empty, visit.Id);
        Assert.Equal(VisitStatus.CheckedIn, visit.Status);
        Assert.NotNull(visit.CheckedInAt);
    }

    [Fact]
    public async Task WalkIn_ReuseExistingVisitor_WhenEmailMatches()
    {
        // First visit
        await _sut.WalkInAsync("John Doe", "john@example.com", "555-1234", null, "Meeting", null);

        // Second visit — same email, updated name
        await _sut.WalkInAsync("John Doe Jr", "john@example.com", "555-9999", null, "Delivery", null);

        var visitors = await _db.Visitors.IgnoreQueryFilters().ToListAsync();
        Assert.Single(visitors);
        Assert.Equal("John Doe Jr", visitors[0].FullName);
    }

    [Fact]
    public async Task WalkIn_SavesNotes()
    {
        var visit = await _sut.WalkInAsync("Jane Smith", "jane@example.com", "555-5678",
            "Acme Corp", "Maintenance", null, "Gate 3");

        Assert.Equal("Gate 3", visit.Notes);
    }

    [Fact]
    public async Task WalkIn_SavesPhotoUrl()
    {
        var visit = await _sut.WalkInAsync("Jane Smith", "jane@example.com", "555-5678",
            null, "Delivery", null, null, "/uploads/test/photo.jpg");

        var visitor = await _db.Visitors.FindAsync(visit.VisitorId);
        Assert.Equal("/uploads/test/photo.jpg", visitor!.PhotoUrl);
    }

    // ── PreRegister ──────────────────────────────────────────────────────

    [Fact]
    public async Task PreRegister_CreatesScheduledVisit()
    {
        var scheduled = DateTime.UtcNow.AddDays(1);

        var visit = await _sut.PreRegisterAsync("Alice Brown", "alice@example.com", "555-0001",
            null, "Interview", null, scheduled);

        Assert.Equal(VisitStatus.Scheduled, visit.Status);
        Assert.Null(visit.CheckedInAt);
    }

    // ── CheckIn ──────────────────────────────────────────────────────────

    [Fact]
    public async Task CheckIn_UpdatesStatusAndTimestamp()
    {
        var visit = await _sut.PreRegisterAsync("Bob White", "bob@example.com", "555-0002",
            null, "Meeting", null, DateTime.UtcNow.AddHours(2));

        await _sut.CheckInAsync(visit.Id);

        var updated = await _db.Visits.FindAsync(visit.Id);
        Assert.Equal(VisitStatus.CheckedIn, updated!.Status);
        Assert.NotNull(updated.CheckedInAt);
    }

    // ── CheckOut ─────────────────────────────────────────────────────────

    [Fact]
    public async Task CheckOut_UpdatesStatusAndTimestamp()
    {
        var visit = await _sut.WalkInAsync("Carol Green", "carol@example.com", "555-0003",
            null, "Delivery", null);

        await _sut.CheckOutAsync(visit.Id);

        var updated = await _db.Visits.FindAsync(visit.Id);
        Assert.Equal(VisitStatus.CheckedOut, updated!.Status);
        Assert.NotNull(updated.CheckedOutAt);
    }

    // ── Cancel ───────────────────────────────────────────────────────────

    [Fact]
    public async Task Cancel_SetsStatusCancelled()
    {
        var visit = await _sut.PreRegisterAsync("Dave Black", "dave@example.com", "555-0004",
            null, "Tour", null, DateTime.UtcNow.AddDays(3));

        await _sut.CancelAsync(visit.Id);

        var updated = await _db.Visits.FindAsync(visit.Id);
        Assert.Equal(VisitStatus.Cancelled, updated!.Status);
    }

    // ── NoShow ───────────────────────────────────────────────────────────

    [Fact]
    public async Task MarkNoShow_SetsStatusNoShow()
    {
        var visit = await _sut.PreRegisterAsync("Eve Gray", "eve@example.com", "555-0005",
            null, "Interview", null, DateTime.UtcNow.AddDays(1));

        await _sut.MarkNoShowAsync(visit.Id);

        var updated = await _db.Visits.FindAsync(visit.Id);
        Assert.Equal(VisitStatus.NoShow, updated!.Status);
    }

    // ── GetVisits pagination ──────────────────────────────────────────────

    [Fact]
    public async Task GetVisits_ReturnsPaginatedResults()
    {
        for (var i = 0; i < 30; i++)
        {
            await _sut.WalkInAsync($"Visitor {i}", $"v{i}@example.com", $"555-{i:0000}",
                null, "Meeting", null);
        }

        var (items, total) = await _sut.GetVisitsAsync("all", null, page: 1, pageSize: 10);

        Assert.Equal(30, total);
        Assert.Equal(10, items.Count);
    }

    [Fact]
    public async Task GetVisits_FiltersActiveVisits()
    {
        await _sut.WalkInAsync("Active One", "a1@example.com", "555-1001", null, "Meeting", null);
        await _sut.PreRegisterAsync("Scheduled One", "s1@example.com", "555-1002",
            null, "Interview", null, DateTime.UtcNow.AddDays(1));

        var (items, _) = await _sut.GetVisitsAsync("active", null);

        Assert.All(items, v => Assert.Equal(VisitStatus.CheckedIn, v.Status));
    }

    public void Dispose() => _db.Dispose();
}
