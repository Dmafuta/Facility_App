using FacilityApp.Data;
using FacilityApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FacilityApp.Services;

public class DashboardService : IDashboardService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public DashboardService(IDbContextFactory<AppDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<DashboardStats> GetStatsAsync()
    {
        await using var _context = await _factory.CreateDbContextAsync();
        var now = DateTime.UtcNow;
        var today = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var monthStart = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var next24h = now.AddHours(24);

        var stats = new DashboardStats
        {
            TodayExpected = await _context.Visits
                .CountAsync(v => v.ScheduledAt >= today && v.ScheduledAt < today.AddDays(1)),

            TodayCheckedIn = await _context.Visits
                .CountAsync(v => v.CheckedInAt >= today && v.CheckedInAt < today.AddDays(1)),

            TodayCheckedOut = await _context.Visits
                .CountAsync(v => v.CheckedOutAt >= today && v.CheckedOutAt < today.AddDays(1)),

            CurrentlyInside = await _context.Visits
                .CountAsync(v => v.Status == VisitStatus.CheckedIn),

            WeekTotal = await _context.Visits
                .CountAsync(v => v.ScheduledAt >= weekStart),

            MonthTotal = await _context.Visits
                .CountAsync(v => v.ScheduledAt >= monthStart),

            UpcomingVisits = await _context.Visits
                .Include(v => v.Visitor)
                .Include(v => v.Host)
                .Where(v => v.Status == VisitStatus.Scheduled
                         && v.ScheduledAt >= now
                         && v.ScheduledAt <= next24h)
                .OrderBy(v => v.ScheduledAt)
                .Take(10)
                .ToListAsync(),

            RecentActivity = await _context.Visits
                .Include(v => v.Visitor)
                .Include(v => v.EntryEntrance)
                .Include(v => v.ExitEntrance)
                .Where(v => v.Status == VisitStatus.CheckedIn || v.Status == VisitStatus.CheckedOut)
                .OrderByDescending(v => v.CheckedInAt ?? v.CheckedOutAt)
                .Take(12)
                .ToListAsync()
        };

        // 30-day daily check-in trend
        var trendStart = today.AddDays(-29);
        var rawTrend = await _context.Visits
            .Where(v => v.CheckedInAt >= trendStart && v.CheckedInAt < today.AddDays(1))
            .GroupBy(v => v.CheckedInAt!.Value.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .ToListAsync();

        var trendDict = rawTrend.ToDictionary(x => DateOnly.FromDateTime(x.Date), x => x.Count);
        stats.DailyTrend = Enumerable.Range(0, 30)
            .Select(i => DateOnly.FromDateTime(trendStart.AddDays(i)))
            .Select(d => (d, trendDict.TryGetValue(d, out var c) ? c : 0))
            .ToList();

        return stats;
    }
}
