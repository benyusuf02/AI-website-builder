namespace YDeveloper.Models.Templates
{
    public class SiteTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string PreviewImageUrl { get; set; } = string.Empty;
        public string TemplateContent { get; set; } = string.Empty;
        public bool IsPremium { get; set; }
        public decimal Price { get; set; }
    }

    public class PageTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string HtmlContent { get; set; } = string.Empty;
        public string CssContent { get; set; } = string.Empty;
        public string JsContent { get; set; } = string.Empty;
    }
}
