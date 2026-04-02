namespace YDeveloper.Models.Webhooks
{
    public class WebhookEvent
    {
        public string EventType { get; set; } = string.Empty;
        public string TargetUrl { get; set; } = string.Empty;
        public object Payload { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int RetryCount { get; set; } = 0;
    }

    public class WebhookSubscription
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string TargetUrl { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
