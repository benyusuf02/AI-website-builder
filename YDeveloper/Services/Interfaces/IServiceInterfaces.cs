namespace YDeveloper.Services.Interfaces
{
    public interface IEmailTemplateService
    {
        Task<string> RenderTemplateAsync(string templateName, Dictionary<string, string> placeholders);
        Task SendWelcomeEmailAsync(string email, string userName, string dashboardUrl);
        Task SendPasswordResetEmailAsync(string email, string resetUrl);
        Task SendPaymentSuccessEmailAsync(string email, decimal amount, string packageName);
    }

    public interface IAnalyticsService
    {
        Task TrackPageViewAsync(int siteId, string path, string? userAgent, string? ipAddress);
        Task<int> GetTodayVisitorsAsync(int siteId);
        Task<int> GetMonthVisitorsAsync(int siteId);
        Task<Dictionary<string, int>> GetTopPagesAsync(int siteId, int count = 10);
    }

    public interface IBackupService
    {
        Task<string> CreateBackupAsync(int siteId);
        Task<bool> RestoreBackupAsync(int siteId, string backupId);
        Task<List<string>> ListBackupsAsync(int siteId);
        Task DeleteBackupAsync(string backupId);
    }
}
