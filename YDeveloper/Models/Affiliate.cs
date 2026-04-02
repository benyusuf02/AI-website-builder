using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YDeveloper.Models
{
    public class Affiliate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Required]
        [MaxLength(20)]
        public string ReferralCode { get; set; } = string.Empty; // e.g. "AHMET123"

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0; // Current withdrawable

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalEarnings { get; set; } = 0; // Lifetime

        public int ClickCount { get; set; } = 0;
        public int ConversionCount { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class AffiliateClick
    {
        public int Id { get; set; }
        public int AffiliateId { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class AffiliateConversion
    {
        public int Id { get; set; }
        public int AffiliateId { get; set; }

        public int PaymentTransactionId { get; set; }
        [ForeignKey("PaymentTransactionId")]
        public PaymentTransaction? Transaction { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; } // Transaction Amount

        [Column(TypeName = "decimal(18,2)")]
        public decimal Commission { get; set; } // Earned

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
