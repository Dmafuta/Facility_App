using FacilityApp.Data.Models;

namespace FacilityApp.Services;

public record DailyCount(DateOnly Date, int Total, int CheckedIn, int NoShow);
public record HourlyCount(int Hour, int Count);
public record VisitorFrequency(string FullName, string Email, int Visits);

public class ReportStats
{
    public int TotalVisits      { get; set; }
    public int TotalCheckedIn   { get; set; }
    public int TotalCheckedOut  { get; set; }
    public int TotalScheduled   { get; set; }
    public int TotalCancelled   { get; set; }
    public int TotalNoShow      { get; set; }
    public double CheckInRate   { get; set; }
    public double AvgPerDay     { get; set; }
    public List<DailyCount>        DailyBreakdown  { get; set; } = [];
    public List<HourlyCount>       HourlyBreakdown { get; set; } = [];
    public List<VisitorFrequency>  TopVisitors     { get; set; } = [];
}

public interface IReportService
{
    Task<ReportStats> GetStatsAsync(DateOnly from, DateOnly to);
    Task<byte[]> GetCsvBytesAsync(DateOnly from, DateOnly to);
}
