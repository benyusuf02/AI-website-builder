using System;
using System.ComponentModel.DataAnnotations;

namespace YDeveloper.Models
{
    public class UserNote
    {
        public int Id { get; set; }

        public string TargetUserId { get; set; } = string.Empty; // The customer
        public string AuthorId { get; set; } = string.Empty; // The moderator

        public string Note { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
