using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YDeveloper.Models
{
    public class BlogPost
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Slug { get; set; } = string.Empty; // e.g. "new-features-released"

        public string? Summary { get; set; } // Short description for listings

        [Required]
        public string Content { get; set; } = string.Empty; // HTML Content

        public string? CoverImageUrl { get; set; }

        public bool IsPublished { get; set; } = false;
        public DateTime? PublishedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Author (Usually Admin/Moderator)
        public string? AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public virtual ApplicationUser? Author { get; set; }

        // SEO
        [MaxLength(200)]
        public string? MetaTitle { get; set; }
        [MaxLength(300)]
        public string? MetaDescription { get; set; }
        public string? Tags { get; set; } // Comma separated tags
    }
}
