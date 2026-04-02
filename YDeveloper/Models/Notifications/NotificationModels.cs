namespace YDeveloper.Models.Notifications
{
    public class NotificationPreferences
    {
        public bool EmailNotifications { get; set; } = true;
        public bool SmsNotifications { get; set; } = false;
        public bool PushNotifications { get; set; } = true;
        public bool MarketingEmails { get; set; } = false;
    }

    public class NotificationTemplate
    {
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, string> Data { get; set; } = new();
    }
}
