namespace YDeveloper.Configuration
{
    public class AppSettings
    {
        public string MainDomain { get; set; } = "ydeveloper.com";
        public bool MaintenanceMode { get; set; } = false;
        public int MaxSitesPerUser { get; set; } = 10;
        public int MaxPagesPerSite { get; set; } = 50;
    }

    public class AiSettings
    {
        public string GeminiApiKey { get; set; } = string.Empty;
        public string ModelName { get; set; } = "gemini-2.5-flash";
        public int TimeoutMinutes { get; set; } = 5;
    }

    public class CacheSettings
    {
        public int DefaultCacheMinutes { get; set; } = 60;
        public bool EnableDistributedCache { get; set; } = true;
    }

    public class SecuritySettings
    {
        public int MaxLoginAttempts { get; set; } = 5;
        public int LockoutMinutes { get; set; } = 15;
        public bool RequireEmailConfirmation { get; set; } = true;
    }
}
