using FacilityApp.Data.Models;

namespace FacilityApp.Services;

public class DashboardStats
{
    public int TodayExpected { get; set; }
    public int TodayCheckedIn { get; set; }
    public int TodayCheckedOut { get; set; }
    public int CurrentlyInside { get; set; }
    public int WeekTotal { get; set; }
    public int MonthTotal { get; set; }
    public List<Visit> UpcomingVisits { get; set; } = [];
    public List<Visit> RecentActivity { get; set; } = [];
    /// <summary>30-day daily check-in counts (oldest first).</summary>
    public List<(DateOnly Date, int Count)> DailyTrend { get; set; } = [];
}

public interface IDashboardService
{
    Task<DashboardStats> GetStatsAsync();
}
