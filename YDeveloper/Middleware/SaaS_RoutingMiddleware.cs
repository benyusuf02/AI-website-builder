using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using YDeveloper.Data;

namespace YDeveloper.Middleware
{
    public class SaaS_RoutingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDistributedCache _cache;
        private readonly string _mainDomain;

        public SaaS_RoutingMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory, IConfiguration configuration, IHttpClientFactory httpClientFactory, IDistributedCache cache)
        {
            _next = next;
            _scopeFactory = scopeFactory;
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _mainDomain = configuration["SystemSettings:MainDomain"] ?? "localhost";
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var host = context.Request.Host.Host;
            var path = context.Request.Path.Value?.ToLower();

            // 0. Bypass SignalR Hubs & Assets in Main Domain
            if (path != null && (path.StartsWith("/chathub") || path.StartsWith("/hubs") || path.StartsWith("/lib") || path.StartsWith("/css") || path.StartsWith("/js")))
            {
                await _next(context);
                return;
            }

            // 1. If accessing main domain, skip logic
            if (host.Equals(_mainDomain, StringComparison.OrdinalIgnoreCase) || host.Equals("www." + _mainDomain, StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            // 2. Identify Tenant & Cache Check
            string cacheKey = $"site_info_{host}";
            var cachedData = await _cache.GetStringAsync(cacheKey);
            SiteCacheModel? siteInfo = null;

            if (cachedData != null)
            {
                siteInfo = JsonSerializer.Deserialize<SiteCacheModel>(cachedData);
            }
            else
            {
                // Cache Miss -> DB Lookup
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<YDeveloperContext>();

                string? subdomain = null;
                string? customDomain = null;

                if (host.EndsWith("." + _mainDomain))
                {
                    subdomain = host.Replace("." + _mainDomain, "");
                }
                else
                {
                    customDomain = host.Replace("www.", "");
                }

                Models.Site? site = null;
                if (!string.IsNullOrEmpty(subdomain)) site = dbContext.Sites.FirstOrDefault(s => s.Subdomain == subdomain);
                else if (!string.IsNullOrEmpty(customDomain)) site = dbContext.Sites.FirstOrDefault(s => s.Domain == customDomain);

                if (site != null)
                {
                    siteInfo = new SiteCacheModel
                    {
                        Id = site.Id,
                        S3Url = site.S3Url,
                        IsActive = site.IsActive,
                        IsPublished = site.IsPublished,
                        SubscriptionEndDate = site.SubscriptionEndDate
                    };

                    // Cache for 1 hour to prevent constant lookups
                    await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(siteInfo), new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
                    });
                }
            }

            // 3. Validation & Content Serving
            if (siteInfo == null || !siteInfo.IsActive || DateTime.UtcNow > siteInfo.SubscriptionEndDate || !siteInfo.IsPublished || string.IsNullOrEmpty(siteInfo.S3Url))
            {
                if (IsResourcePath(path)) { context.Response.StatusCode = 404; return; }
                await ServeParkPage(context);
                return;
            }

            // 4. Content Serving Logic (Reverse Proxy with S3 Optimized Path)
            try
            {
                string fileUrl = siteInfo.S3Url!;
                if (!fileUrl.EndsWith("/")) fileUrl += "/";

                string targetFile = (path ?? "").TrimStart('/').Trim();

                if (string.IsNullOrEmpty(targetFile)) targetFile = "index.html";
                else if (!Path.HasExtension(targetFile)) targetFile += ".html";

                string targetUrl = fileUrl + targetFile;

                // PERFORMANCE FIX: If it's a large asset (not HTML), we should ideally redirect to S3 directly 
                // but if we want to hide S3 URL, we proxy it. 
                // For now, we fix the "every request hits DB" part, which is the biggest bottleneck.

                var httpClient = _httpClientFactory.CreateClient();
                // Add conditional cache headers for the proxy client if needed
                var response = await httpClient.GetAsync(targetUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsByteArrayAsync();
                    var contentType = response.Content.Headers.ContentType?.ToString() ?? "text/html";
                    context.Response.ContentType = contentType;

                    // SECURITY SHIELD: Content Security Policy
                    // Blocks untrusted scripts, prevents clickjacking, and enforces HTTPS
                    if (contentType.Contains("text/html"))
                    {
                        context.Response.Headers.Append("Content-Security-Policy",
                            "default-src 'self' https:; " +
                            "script-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://unpkg.com https://www.google-analytics.com; " +
                            "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
                            "img-src 'self' data: https:; " +
                            "font-src 'self' https://fonts.gstatic.com https://cdnjs.cloudflare.com; " +
                            "connect-src 'self' https://ka-f.fontawesome.com; " +
                            "frame-ancestors 'none'; " + // Prevent Clickjacking
                            "base-uri 'self';");

                        context.Response.Headers.Append("X-Frame-Options", "DENY");
                        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
                    }

                    // Add public caching for assets served via proxy
                    if (IsResourcePath(path))
                    {
                        context.Response.Headers.Append("Cache-Control", "public,max-age=86400"); // 1 day cache
                    }

                    await context.Response.Body.WriteAsync(content, 0, content.Length);
                }
                else
                {
                    context.Response.StatusCode = 404;
                    if (targetFile.EndsWith(".html")) await context.Response.WriteAsync("Sayfa Bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync($"Hata: {ex.Message}");
            }
        }

        private async Task ServeParkPage(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            string content = "<html><body style='font-family:sans-serif;text-align:center;padding-top:100px;'><h1>Site Hazırlanıyor</h1><p>YDeveloper.com</p></body></html>";
            await context.Response.WriteAsync(content);
        }

        private bool IsResourcePath(string? path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            var ext = Path.GetExtension(path).ToLower();
            return !string.IsNullOrEmpty(ext) && ext != ".html";
        }

        private class SiteCacheModel
        {
            public int Id { get; set; }
            public string? S3Url { get; set; }
            public bool IsActive { get; set; }
            public bool IsPublished { get; set; }
            public DateTime SubscriptionEndDate { get; set; }
        }
    }
}
