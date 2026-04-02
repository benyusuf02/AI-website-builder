using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using YDeveloper.Data;

namespace YDeveloper.Middleware
{
    public class DomainRoutingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _parkPagePath;
        private readonly string _mainDomain;

        public DomainRoutingMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _parkPagePath = configuration["SystemSettings:ParkPageHtmlPath"] ?? "wwwroot/templates/park-page.html";
            _mainDomain = configuration["SystemSettings:MainDomain"] ?? "localhost";
        }

        public async Task InvokeAsync(HttpContext context, YDeveloperContext dbContext)
        {
            var host = context.Request.Host.Host.ToLowerInvariant();
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? "/";

            // Skip for main domain or system paths
            if (host.Contains(_mainDomain) ||
                path.StartsWith("/admin") ||
                path.StartsWith("/account") ||
                path.StartsWith("/dashboard") ||
                path.StartsWith("/identity") ||
                path.StartsWith("/lib") ||
                path.StartsWith("/css") ||
                path.StartsWith("/js"))
            {
                await _next(context);
                return;
            }

            // Clean host
            var cleanHost = host.Replace("www.", "").Replace("https://", "").Replace("http://", "");

            // Check if domain exists in database
            var site = await dbContext.Sites.FirstOrDefaultAsync(s => s.Domain == cleanHost);

            if (site != null)
            {
                if (site.IsActive)
                {
                    // Site is active, check for page
                    var page = await dbContext.Pages.FirstOrDefaultAsync(p => p.SiteId == site.Id && p.Slug == path);

                    if (page != null)
                    {
                        context.Response.ContentType = "text/html; charset=utf-8";
                        await context.Response.WriteAsync(page.HtmlContent);
                        return;
                    }
                    else
                    {
                        // Page not found
                        context.Response.StatusCode = 404;
                        await context.Response.WriteAsync("<h1>404 - Page Not Found</h1>");
                        return;
                    }
                }
                else
                {
                    // Site is inactive, show park page
                    await ServeParkPage(context);
                    return;
                }
            }

            // Domain not found, continue to normal routing
            await _next(context);
        }

        private async Task ServeParkPage(HttpContext context)
        {
            try
            {
                if (File.Exists(_parkPagePath))
                {
                    var parkPageContent = await File.ReadAllTextAsync(_parkPagePath);
                    context.Response.ContentType = "text/html; charset=utf-8";
                    await context.Response.WriteAsync(parkPageContent);
                }
                else
                {
                    // Fallback park page
                    context.Response.ContentType = "text/html; charset=utf-8";
                    await context.Response.WriteAsync(@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <title>Coming Soon</title>
                            <style>
                                body { font-family: sans-serif; text-align: center; padding-top: 100px; }
                                h1 { color: #667eea; }
                            </style>
                        </head>
                        <body>
                            <h1>🚀 Coming Soon</h1>
                            <p>This website is under construction.</p>
                        </body>
                        </html>
                    ");
                }
            }
            catch
            {
                context.Response.StatusCode = 503;
                await context.Response.WriteAsync("Service Temporarily Unavailable");
            }
        }
    }
}
