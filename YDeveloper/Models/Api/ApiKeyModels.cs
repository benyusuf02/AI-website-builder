namespace YDeveloper.Models.Api
{
    public class ApiKey
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
    }

    public class RateLimitInfo
    {
        public int Limit { get; set; }
        public int Remaining { get; set; }
        public DateTime ResetAt { get; set; }
    }
}
