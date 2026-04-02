using System;
using System.ComponentModel.DataAnnotations;

namespace YDeveloper.Models
{
    public class Coupon
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; } = string.Empty; // e.g. "SUMMER2025"

        [Range(0, 100)]
        public int DiscountPercentage { get; set; } // e.g., 20

        public DateTime ExpiryDate { get; set; }

        public int MaxUses { get; set; } = 100;

        public int CurrentUses { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(200)]
        public string? Description { get; set; }
    }
}
