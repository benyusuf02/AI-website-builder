using System.ComponentModel.DataAnnotations;

namespace YDeveloper.Models
{
    public class ContentItem
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Section { get; set; } = "homepage";
        public string Category { get; set; } = "text";
        public string Description { get; set; } = string.Empty;
        public bool IsEditable { get; set; } = true;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Legacy properties for backward compatibility
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Icon { get; set; }
        public string? ImageUrl { get; set; }
        public string? ButtonText { get; set; }
        public string? ButtonUrl { get; set; }
        public int Order { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        [MaxLength]
        public string HtmlContent { get; set; } = string.Empty;
    }
}
