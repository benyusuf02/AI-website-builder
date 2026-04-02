using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YDeveloper.Models
{
    public enum TicketStatus
    {
        Open,
        Pending,
        Resolved,
        Escalated, // To Admin
        Closed
    }

    public enum TicketPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public class Ticket
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Subject { get; set; } = string.Empty;

        public TicketStatus Status { get; set; } = TicketStatus.Open;

        public TicketPriority Priority { get; set; } = TicketPriority.Medium;

        [Required]
        public string CreatorId { get; set; } = string.Empty;
        public ApplicationUser? Creator { get; set; }

        public string? AssignedModeratorId { get; set; }
        public ApplicationUser? AssignedModerator { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public List<TicketMessage> Messages { get; set; } = new List<TicketMessage>();
        public List<TicketAttachment> Attachments { get; set; } = new List<TicketAttachment>();
    }
}
