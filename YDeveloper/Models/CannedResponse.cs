using System.ComponentModel.DataAnnotations;

namespace YDeveloper.Models
{
    public class CannedResponse
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty; // e.g., "Welcome", "Payment Issue"

        [Required]
        public string Content { get; set; } = string.Empty; // Full text

        public string? Category { get; set; } // e.g. "Billing", "Technical"
        public bool IsActive { get; set; } = true;
    }
}
