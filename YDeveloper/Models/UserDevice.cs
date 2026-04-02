using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YDeveloper.Models
{
    public class UserDevice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        public string? DeviceName { get; set; } // e.g. "Chrome on Windows"
        public string? IpAddress { get; set; }
        public string? Location { get; set; } // e.g. "US, CA" (IP-based)

        public DateTime LastActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // UserAgent string for raw storage
        public string? UserAgent { get; set; }
    }
}
