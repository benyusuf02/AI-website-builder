namespace YDeveloper.Models.Api
{
    public class DashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveSubscriptions { get; set; }
        public decimal EstimatedMonthlyRevenue { get; set; }
        public int NewUsersToday { get; set; }
        public int WarningCount { get; set; }
    }
}
