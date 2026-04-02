namespace YDeveloper.Models.Reports
{
    public class SalesReport
    {
        public decimal TotalRevenue { get; set; }
        public int TotalTransactions { get; set; }
        public decimal AverageOrderValue { get; set; }
        public Dictionary<string, decimal> RevenueByPackage { get; set; } = new();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public class UsageReport
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalSites { get; set; }
        public int TotalPages { get; set; }
        public long TotalVisits { get; set; }
        public Dictionary<string, int> UsersByPackage { get; set; } = new();
    }

    public class PerformanceReport
    {
        public double AverageResponseTime { get; set; }
        public int TotalRequests { get; set; }
        public int ErrorCount { get; set; }
        public double ErrorRate { get; set; }
        public Dictionary<string, double> SlowEndpoints { get; set; } = new();
    }
}
