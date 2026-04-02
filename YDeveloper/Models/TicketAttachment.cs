using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YDeveloper.Models
{
    public class TicketAttachment
    {
        public int Id { get; set; }

        public int TicketId { get; set; }
        [ForeignKey("TicketId")]
        public Ticket? Ticket { get; set; }

        public int? TicketMessageId { get; set; }
        [ForeignKey("TicketMessageId")]
        public TicketMessage? TicketMessage { get; set; }

        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string FileUrl { get; set; } = string.Empty; // S3 Public or Presigned URL

        public string? ContentType { get; set; } // e.g., image/png, application/pdf

        public long SizeBytes { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
