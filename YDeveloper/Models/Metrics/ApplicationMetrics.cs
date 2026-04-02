namespace YDeveloper.Models.Metrics
{
    public class ApplicationMetrics
    {
        public int TotalUsers { get; set; }
        public int ActiveSites { get; set; }
        public int TotalPages { get; set; }
        public long TotalVisits { get; set; }
        public decimal TotalRevenue { get; set; }
        public double AverageResponseTime { get; set; }
        public int ErrorCount { get; set; }
        public Dictionary<string, int> TopErrors { get; set; } = new();
    }

    public class PerformanceMetric
    {
        public string Endpoint { get; set; } = string.Empty;
        public double AverageResponseTime { get; set; }
        public int RequestCount { get; set; }
        public int ErrorCount { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
