namespace YDeveloper.Models.ViewModels
{
    public class DashboardStatsViewModel
    {
        public int TotalSites { get; set; }
        public int ActiveSites { get; set; }
        public int TotalPages { get; set; }
        public int TotalVisitorsToday { get; set; }
        public int TotalVisitorsMonth { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public List<ChartDataPoint> VisitorChartData { get; set; } = new();
        public List<SiteQuickInfo> RecentSites { get; set; } = new();
    }

    public class ChartDataPoint
    {
        public string Label { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    public class SiteQuickInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int PageCount { get; set; }
    }
}
