namespace YDeveloper.Models.Analytics
{
    public class VisitorLog
    {
        public int Id { get; set; }
        public int SiteId { get; set; }
        public string Path { get; set; } = string.Empty;
        public string? UserAgent { get; set; }
        public string? IpAddress { get; set; }
        public DateTime VisitedAt { get; set; } = DateTime.UtcNow;
    }

    public class PageViewStats
    {
        public string PagePath { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public int UniqueVisitors { get; set; }
        public double AverageTimeOnPage { get; set; }
    }
}
