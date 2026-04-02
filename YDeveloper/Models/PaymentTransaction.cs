using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YDeveloper.Models
{
    public class PaymentTransaction
    {
        [Key]
        public int Id { get; set; }

        public string? TransactionId { get; set; } // Ödeme sağlayıcıdan dönen ID

        [MaxLength(100)]
        public string? ConversationId { get; set; } // Bizim oluşturduğumuz takip ID

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public string Currency { get; set; } = "TRY";

        public string Status { get; set; } = "Pending"; // Success, Failure, Pending

        // Fraud Prevention Fields
        public string? IpAddress { get; set; }

        public bool IsThreeDSecure { get; set; }

        public string? ThreeDSecureInfo { get; set; } // JSON formatında ham 3D verisi veya açıklaması

        public string? ErrorMessage { get; set; } // Hata durumunda detay

        public string? UserAgent { get; set; } // Tarayıcı bilgisi

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
