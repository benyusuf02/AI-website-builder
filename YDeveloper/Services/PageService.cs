using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;
using System.Text.RegularExpressions;

namespace YDeveloper.Services
{
    public class PageService : IPageService
    {
        private readonly YDeveloperContext _context;
        private readonly IS3Service _s3Service;
        private readonly GeminiService _geminiService;
        private readonly IConfiguration _configuration;

        public PageService(YDeveloperContext context, IS3Service s3Service, GeminiService geminiService, IConfiguration configuration)
        {
            _context = context;
            _s3Service = s3Service;
            _geminiService = geminiService;
            _configuration = configuration;
        }

        public async Task<Page?> GetPageAsync(int pageId, string userId)
        {
            return await _context.Pages
                .Include(p => p.Site)
                .FirstOrDefaultAsync(p => p.Id == pageId && p.Site != null && p.Site.UserId == userId);
        }

        public async Task<Page> CreatePageAsync(int siteId, string userId, string pageName, string slug, string prompt)
        {
            var site = await _context.Sites.FirstOrDefaultAsync(s => s.Id == siteId && s.UserId == userId);
            if (site == null) throw new Exception("Site not found or access denied.");

            if (!string.IsNullOrEmpty(slug) && !slug.StartsWith("/")) slug = "/" + slug;
            if (string.IsNullOrEmpty(slug)) slug = "/" + StringHelper.Slugify(pageName);

            // Determine Page Type & Prompt
            string pageType = (slug == "/" || slug == "/index") ? "Landing" : "Inner";
            string aiPrompt = "";
            var existingPage = await _context.Pages.FirstOrDefaultAsync(p => p.SiteId == siteId);

            if (existingPage == null)
            {
                aiPrompt = $@"
Sen üst düzey bir UI/UX uzmanısın.
SİTE KONSEPTİ: {site.DesignPrompt}
GÖREV: '{pageName}' sayfası için HTML oluştur.
İÇERİK: {prompt}
KURALLAR: Sadece HTML. Tailwind CSS. Vanilla JS.
";
            }
            else
            {
                aiPrompt = $@"
Sen profesyonel bir web geliştiricisisin.
REFERANS HTML (Stil için): {existingPage.HtmlContent?.Substring(0, Math.Min(existingPage.HtmlContent?.Length ?? 0, 500))}...
GÖREV: '{pageName}' sayfası oluştur.
İÇERİK: {prompt}
KURALLAR: Referans tasarıma uy. Sadece HTML.
";
            }

            string newHtml = "";
            try { newHtml = await _geminiService.GenerateHtmlAsync(aiPrompt, pageType); }
            catch (Exception ex) { newHtml = $"<h1>AI Hatası: {ex.Message}</h1>"; }

            var newPage = new Page
            {
                SiteId = siteId,
                Slug = slug,
                MetaTitle = pageName,
                MetaDescription = pageName,
                HtmlContent = newHtml
            };

            _context.Pages.Add(newPage);
            await _context.SaveChangesAsync();

            // Sync Menus
            await SyncMenusAsync(siteId);

            return newPage;
        }

        public async Task<bool> UpdateContentAsync(int pageId, string userId, string htmlContent)
        {
            if (htmlContent.Contains("var dbContent =") || htmlContent.Contains("grapesjs.init")) return false;

            var page = await GetPageAsync(pageId, userId);
            if (page == null) return false;

            page.HtmlContent = htmlContent;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePageAsync(int pageId, string userId)
        {
            var page = await GetPageAsync(pageId, userId);
            if (page == null) return false;

            int siteId = page.SiteId;
            _context.Pages.Remove(page);
            await _context.SaveChangesAsync();
            await SyncMenusAsync(siteId);
            return true;
        }

        public async Task<string> PublishPageAsync(int pageId, string userId, string? htmlContent = null)
        {
            var page = await GetPageAsync(pageId, userId);
            if (page == null) throw new Exception("Page not found");

            if (!string.IsNullOrEmpty(htmlContent))
            {
                await UpdateContentAsync(pageId, userId, htmlContent);
                page = await GetPageAsync(pageId, userId); // Refresh
            }

            if (string.IsNullOrEmpty(page.HtmlContent)) throw new Exception("Page content is empty");
            if (page.Site == null) throw new Exception("Site not found");

            var bucketName = _configuration["AWS:BucketName"] ?? throw new Exception("Bucket config missing");

            string slug = page.Slug?.Trim().ToLower() ?? "index";
            if (slug == "/" || string.IsNullOrEmpty(slug)) slug = "index";
            if (slug.StartsWith("/")) slug = slug.Substring(1);

            string fileName = $"sites/{page.SiteId}/{slug}.html";
            string s3FileUrl = await _s3Service.UploadFileAsync(bucketName, fileName, page.HtmlContent, "text/html");

            string siteRootUrl = s3FileUrl.Substring(0, s3FileUrl.LastIndexOf('/') + 1);
            page.Site.S3Url = siteRootUrl;
            page.Site.IsPublished = true;
            page.Site.IsActive = true;
            page.PublishedUrl = s3FileUrl;
            page.LastPublishedAt = DateTime.UtcNow;

            _context.Sites.Update(page.Site);
            _context.Pages.Update(page);
            await _context.SaveChangesAsync();

            // Construct Final URL
            if (!string.IsNullOrEmpty(page.Site.Domain))
            {
                string finalUrl = $"http://{page.Site.Domain}/{slug}";
                if (slug == "index") finalUrl = $"http://{page.Site.Domain}/";
                return finalUrl;
            }
            else
            {
                var mainDomain = _configuration["SystemSettings:MainDomain"] ?? "ydeveloper.com";
                string finalUrl = $"http://{page.Site.Subdomain}.{mainDomain}/{slug}";
                if (slug == "index") finalUrl = $"http://{page.Site.Subdomain}.{mainDomain}/";
                return finalUrl;
            }
        }

        public async Task SyncMenusAsync(int siteId)
        {
            var allPages = await _context.Pages
                .Where(p => p.SiteId == siteId)
                .OrderBy(p => p.Id)
                .ToListAsync();

            if (!allPages.Any()) return;

            // Generate Menu HTML
            var sb = new System.Text.StringBuilder();
            foreach (var p in allPages)
            {
                string title = p.MetaTitle ?? p.Slug?.Trim('/') ?? "Link";
                if (string.IsNullOrEmpty(title)) title = "Sayfa";
                if (p.Slug == "/" || p.Slug == "/index" || string.IsNullOrEmpty(p.Slug)) title = "Ana Sayfa";

                string url = (p.Slug == "/" || p.Slug == "/index" || string.IsNullOrEmpty(p.Slug))
                    ? "index.html"
                    : $"{p.Slug.TrimStart('/')}.html";

                sb.AppendLine($"<li class=\"nav-item\"><a class=\"nav-link\" href=\"{url}\">{title}</a></li>");
            }
            string menuHtml = sb.ToString();

            // Update All Pages
            var bucketName = _configuration["AWS:BucketName"];

            foreach (var p in allPages)
            {
                if (string.IsNullOrEmpty(p.HtmlContent)) continue;
                string content = p.HtmlContent;
                bool isModified = false;

                if (content.Contains("id=\"site-menu\""))
                {
                    var pattern = @"(<ul[^>]*id=""site-menu""[^>]*>)(.*?)(</ul>)";
                    var newContent = Regex.Replace(content, pattern, $"$1{menuHtml}$3", RegexOptions.Singleline | RegexOptions.IgnoreCase);

                    if (newContent != content)
                    {
                        content = newContent;
                        isModified = true;
                    }
                }

                if (content.Contains("__SITE_ID__"))
                {
                    content = content.Replace("__SITE_ID__", siteId.ToString());
                    isModified = true;
                }

                if (isModified)
                {
                    p.HtmlContent = content;
                    if (!string.IsNullOrEmpty(p.PublishedUrl) && !string.IsNullOrEmpty(bucketName))
                    {
                        string slug = p.Slug?.Trim().ToLower() ?? "index";
                        if (slug == "/" || string.IsNullOrEmpty(slug)) slug = "index";
                        if (slug.StartsWith("/")) slug = slug.Substring(1);
                        string fileName = $"sites/{p.SiteId}/{slug}.html";
                        try { await _s3Service.UploadFileAsync(bucketName, fileName, content, "text/html"); } catch { }
                    }
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
