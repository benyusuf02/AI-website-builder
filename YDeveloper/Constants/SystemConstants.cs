namespace YDeveloper.Constants
{
    public static class LoggingConstants
    {
        public const string RequestStarted = "Request started";
        public const string RequestCompleted = "Request completed";
        public const string ErrorOccurred = "Error occurred";
    }

    public static class EventNames
    {
        public const string SiteCreated = "Site.Created";
        public const string PagePublished = "Page.Published";
        public const string PaymentCompleted = "Payment.Completed";
        public const string UserRegistered = "User.Registered";
    }

    public static class QueueNames
    {
        public const string EmailQueue = "email-queue";
        public const string AnalyticsQueue = "analytics-queue";
        public const string BackupQueue = "backup-queue";
    }
}
