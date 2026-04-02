namespace YDeveloper.Models.Events
{
    public abstract class DomainEvent
    {
        public DateTime OccurredAt { get; } = DateTime.UtcNow;
        public string EventType => GetType().Name;
    }

    public class SiteCreatedEvent : DomainEvent
    {
        public int SiteId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

    public class PagePublishedEvent : DomainEvent
    {
        public int PageId { get; set; }
        public int SiteId { get; set; }
        public string Url { get; set; } = string.Empty;
    }

    public class PaymentCompletedEvent : DomainEvent
    {
        public string UserId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PackageType { get; set; } = string.Empty;
    }
}
