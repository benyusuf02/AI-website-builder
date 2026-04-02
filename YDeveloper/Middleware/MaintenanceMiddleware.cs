using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace YDeveloper.Middleware
{
    public class MaintenanceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _cache;

        public MaintenanceMiddleware(RequestDelegate next, IDistributedCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check Redis/Cache for "MaintenanceMode" key
            var isMaintenance = await _cache.GetStringAsync("MaintenanceMode");

            if (!string.IsNullOrEmpty(isMaintenance) && isMaintenance == "true")
            {
                // Allow Admin area or Login/Logout to accessible
                var path = context.Request.Path.Value?.ToLower();
                if (path != null && (
                    path.StartsWith("/admin") ||
                    path.StartsWith("/identity") ||
                    path.Contains("login") ||
                    path.Contains("logout")))
                {
                    await _next(context);
                    return;
                }

                // Return 503 Service Unavailable with friendly message
                context.Response.StatusCode = 503;
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync(@"
                    <html>
                    <head><title>Bakım Modu</title>
                    <style>body{font-family:sans-serif;text-align:center;padding:50px;}</style>
                    </head>
                    <body>
                        <h1>🛠️ Sistem Bakımda</h1>
                        <p>Şu anda planlı bir bakım çalışması yapıyoruz. Lütfen daha sonra tekrar deneyiniz.</p>
                        <p>We are currently undergoing scheduled maintenance. Please try again later.</p>
                    </body>
                    </html>");
                return;
            }

            await _next(context);
        }
    }
}
