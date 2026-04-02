namespace YDeveloper.Enums
{
    public enum SiteStatus
    {
        Draft = 0,
        Active = 1,
        Suspended = 2,
        Deleted = 3,
        Maintenance = 4
    }

    public enum PackageType
    {
        Starter = 0,
        Pro = 1,
        Enterprise = 2
    }

    public enum PaymentStatus
    {
        Pending = 0,
        Success = 1,
        Failed = 2,
        Refunded = 3
    }

    public enum TicketPriority
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Urgent = 3
    }

    public enum TicketStatus
    {
        Open = 0,
        InProgress = 1,
        Waiting = 2,
        Resolved = 3,
        Closed = 4
    }
}
