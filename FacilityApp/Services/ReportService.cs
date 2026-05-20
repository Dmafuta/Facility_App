using System.Text;
using FacilityApp.Data;
using FacilityApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FacilityApp.Services;

public class ReportService : IReportService
{
    private readonly IDbContextFactory<AppDbContext> _factory;

    public ReportService(IDbContextFactory<AppDbContext> factory) => _factory = factory;

    public async Task<ReportStats> GetStatsAsync(DateOnly from, DateOnly to)
    {
        await using var _context = await _factory.CreateDbContextAsync();
        var fromUtc = from.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var toUtc   = to.ToDateTime(new TimeOnly(23, 59, 59), DateTimeKind.Utc);

        var visits = await _context.Visits
            .Include(v => v.Visitor)
            .Where(v => v.ScheduledAt >= fromUtc && v.ScheduledAt <= toUtc)
            .ToListAsync();

        var checkedIn = visits.Where(v =>
            v.Status == VisitStatus.CheckedIn || v.Status == VisitStatus.CheckedOut).ToList();

        var days = Math.Max(1, (to.ToDateTime(TimeOnly.MinValue) - from.ToDateTime(TimeOnly.MinValue)).TotalDays + 1);

        var stats = new ReportStats
        {
            TotalVisits     = visits.Count,
            TotalCheckedIn  = checkedIn.Count,
            TotalCheckedOut = visits.Count(v => v.Status == VisitStatus.CheckedOut),
            TotalScheduled  = visits.Count(v => v.Status == VisitStatus.Scheduled),
            TotalCancelled  = visits.Count(v => v.Status == VisitStatus.Cancelled),
            TotalNoShow     = visits.Count(v => v.Status == VisitStatus.NoShow),
            CheckInRate     = visits.Count > 0 ? Math.Round(checkedIn.Count * 100.0 / visits.Count, 1) : 0,
            AvgPerDay       = Math.Round(visits.Count / days, 1),
        };

        stats.DailyBreakdown = visits
            .GroupBy(v => DateOnly.FromDateTime(v.ScheduledAt.ToLocalTime()))
            .Select(g => new DailyCount(
                g.Key,
                g.Count(),
                g.Count(v => v.Status == VisitStatus.CheckedIn || v.Status == VisitStatus.CheckedOut),
                g.Count(v => v.Status == VisitStatus.NoShow)))
            .OrderBy(d => d.Date)
            .ToList();

        stats.HourlyBreakdown = visits
            .Where(v => v.CheckedInAt.HasValue)
            .GroupBy(v => v.CheckedInAt!.Value.ToLocalTime().Hour)
            .Select(g => new HourlyCount(g.Key, g.Count()))
            .OrderBy(h => h.Hour)
            .ToList();

        stats.TopVisitors = visits
            .GroupBy(v => v.VisitorId)
            .Select(g => new VisitorFrequency(
                g.First().Visitor.FullName,
                g.First().Visitor.Email,
                g.Count()))
            .OrderByDescending(v => v.Visits)
            .Take(10)
            .ToList();

        return stats;
    }

    public async Task<byte[]> GetCsvBytesAsync(DateOnly from, DateOnly to)
    {
        await using var _context = await _factory.CreateDbContextAsync();
        var fromUtc = from.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var toUtc   = to.ToDateTime(new TimeOnly(23, 59, 59), DateTimeKind.Utc);

        var visits = await _context.Visits
            .Include(v => v.Visitor)
            .Include(v => v.Host)
            .Include(v => v.EntryEntrance)
            .Include(v => v.ExitEntrance)
            .Where(v => v.ScheduledAt >= fromUtc && v.ScheduledAt <= toUtc)
            .OrderBy(v => v.ScheduledAt)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("Visitor Name,Email,Phone,Company,Host,Purpose,Scheduled,Checked In,Entry Gate,Checked Out,Exit Gate,Status,Notes");

        foreach (var v in visits)
        {
            sb.AppendLine(string.Join(",",
                Esc(v.Visitor.FullName),
                Esc(v.Visitor.Email),
                Esc(v.Visitor.Phone),
                Esc(v.Visitor.Company ?? ""),
                Esc(v.Host?.FullName ?? ""),
                Esc(v.Purpose),
                v.ScheduledAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
                v.CheckedInAt?.ToLocalTime().ToString("yyyy-MM-dd HH:mm") ?? "",
                Esc(v.EntryEntrance?.Name ?? ""),
                v.CheckedOutAt?.ToLocalTime().ToString("yyyy-MM-dd HH:mm") ?? "",
                Esc(v.ExitEntrance?.Name ?? ""),
                v.Status.ToString(),
                Esc(v.Notes ?? "")));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static string Esc(string s) => $"\"{s.Replace("\"", "\"\"")}\"";
}
