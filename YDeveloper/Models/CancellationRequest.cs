using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YDeveloper.Models
{
    public class CancellationRequest
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string Reason { get; set; } = string.Empty; // e.g. "Too Expensive", "Missing Features"

        public string? Feedback { get; set; } // Detailed explanation

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
