using System;
using System.Collections.Generic;

namespace YDeveloper.Models
{
    public class LiveChatSession
    {
        public int Id { get; set; }

        public string? CustomerUserId { get; set; }
        public ApplicationUser? CustomerUser { get; set; }

        public string? GuestId { get; set; } // For non-logged in users

        public string? ModeratorUserId { get; set; }
        public ApplicationUser? ModeratorUser { get; set; }

        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? EndTime { get; set; }

        public bool IsActive { get; set; } = true;

        public List<LiveChatMessage> Messages { get; set; } = new List<LiveChatMessage>();
    }

    public class LiveChatMessage
    {
        public int Id { get; set; }

        public int LiveChatSessionId { get; set; }
        public LiveChatSession? LiveChatSession { get; set; }

        public string SenderId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
