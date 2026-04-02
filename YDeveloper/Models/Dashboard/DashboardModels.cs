namespace YDeveloper.Models
{
    public class SiteAnalytics
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public DateTime Date { get; set; }
        public int Visitors { get; set; }
        public int PageViews { get; set; }
        public string? Referrer { get; set; }
        public string? DeviceType { get; set; } // Mobile, Desktop, Tablet
        public string? Country { get; set; }

        // Navigation properties
        public Site? Site { get; set; }
    }

    public class DashboardViewModel
    {
        public string? SiteName { get; set; }
        public string? Domain { get; set; }
        public bool IsActive { get; set; }
        public int TotalPages { get; set; }
        public int TodayVisitors { get; set; }
        public int MonthVisitors { get; set; }
        public DateTime? LastUpdated { get; set; }
        public List<ChartDataPoint> VisitorChartData { get; set; } = new();
        public Dictionary<string, int> TopPages { get; set; } = new();
        public Dictionary<string, int> TrafficSources { get; set; } = new();
    }

    public class ChartDataPoint
    {
        public string Label { get; set; } = string.Empty;
        public int Value { get; set; }
    }
}
