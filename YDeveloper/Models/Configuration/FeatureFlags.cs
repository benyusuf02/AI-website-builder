namespace YDeveloper.Models.Configuration
{
    public class FeatureFlags
    {
        public bool EnableNewDashboard { get; set; } = false;
        public bool EnableAiChatbot { get; set; } = false;
        public bool EnableAdvancedAnalytics { get; set; } = false;
        public bool EnableMarketplace { get; set; } = false;
        public bool MaintenanceMode { get; set; } = false;
    }

    public class RateLimitConfig
    {
        public int GlobalLimit { get; set; } = 100;
        public int ApiLimit { get; set; } = 30;
        public int AuthLimit { get; set; } = 5;
        public int WindowMinutes { get; set; } = 1;
    }
}
