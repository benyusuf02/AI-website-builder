using System;
using System.ComponentModel.DataAnnotations;

namespace YDeveloper.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ActionUrl { get; set; } // Link to click
    }
}
