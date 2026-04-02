namespace YDeveloper.Mappers
{
    public static class SiteMapper
    {
        public static DTOs.SiteDto ToDto(Models.Site site)
        {
            return new DTOs.SiteDto
            {
                Id = site.Id,
                Name = site.Subdomain, // Using Subdomain as Name
                Domain = site.Domain ?? string.Empty,
                IsActive = site.IsPublished,
                CreatedAt = site.CreatedAt,
                PageCount = site.Pages?.Count ?? 0
            };
        }

        public static Models.Site ToEntity(DTOs.CreateSiteDto dto, string userId)
        {
            return new Models.Site
            {
                Subdomain = dto.Subdomain,
                UserId = userId,
                IsPublished = true,
                CreatedAt = DateTime.UtcNow
            };
        }
    }

    public static class PageMapper
    {
        public static DTOs.PageDto ToDto(Models.Page page)
        {
            return new DTOs.PageDto
            {
                Id = page.Id,
                Title = page.MetaTitle,
                Slug = page.Slug,
                IsPublished = page.IsPublished,
                LastPublishedAt = page.CreatedAt
            };
        }
    }
}
