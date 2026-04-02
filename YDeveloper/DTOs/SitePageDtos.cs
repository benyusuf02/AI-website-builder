namespace YDeveloper.DTOs
{
    public class SiteDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int PageCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PageDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public DateTime? LastPublishedAt { get; set; }
    }

    public class CreateSiteDto
    {
        public string Name { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public string PackageType { get; set; } = "starter";
    }

    public class UpdatePageDto
    {
        public string? HtmlContent { get; set; }
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
    }
}
