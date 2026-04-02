namespace YDeveloper.Models.Integrations
{
    public class ApiIntegration
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
    }

    public class GoogleAnalyticsConfig
    {
        public string TrackingId { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
    }

    public class MailchimpConfig
    {
        public string ApiKey { get; set; } = string.Empty;
        public string ListId { get; set; } = string.Empty;
    }
}
