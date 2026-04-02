using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YDeveloper.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        [Required]
        public string Action { get; set; } = string.Empty; // e.g., "PasswordReset", "PackageUpdate"

        public string? Details { get; set; } // JSON or text description

        public string? PerformerUserId { get; set; } // Who did it? (Moderator/Admin)
        [ForeignKey("PerformerUserId")]
        public virtual ApplicationUser? PerformerUser { get; set; }

        public string? TargetUserId { get; set; } // Who was it done to? (Customer)
        [ForeignKey("TargetUserId")]
        public virtual ApplicationUser? TargetUser { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string? IpAddress { get; set; }
    }
}
