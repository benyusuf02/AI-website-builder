namespace YDeveloper.Models.Security
{
    public class SecurityAudit
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }
        public string? FailureReason { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class LoginAttempt
    {
        public string Email { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }
        public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
    }

    public class TwoFactorToken
    {
        public string UserId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
    }
}
