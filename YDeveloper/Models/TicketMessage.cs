using System;
using System.ComponentModel.DataAnnotations;

namespace YDeveloper.Models
{
    public class TicketMessage
    {
        public int Id { get; set; }

        public int TicketId { get; set; }
        public Ticket? Ticket { get; set; }

        [Required]
        public string SenderId { get; set; } = string.Empty;
        public ApplicationUser? Sender { get; set; }

        [Required]
        public string Message { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public bool IsInternal { get; set; } = false;
    }
}
