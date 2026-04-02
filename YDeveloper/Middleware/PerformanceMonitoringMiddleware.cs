using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace YDeveloper.Middleware
{
    /// <summary>
    /// Performance monitoring middleware - request duration tracking
    /// </summary>
    public class PerformanceMonitoringMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceMonitoringMiddleware> _logger;

        public PerformanceMonitoringMiddleware(RequestDelegate next, ILogger<PerformanceMonitoringMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            
            try
            {
                await _next(context);
            }
            finally
            {
                sw.Stop();
                
                if (sw.ElapsedMilliseconds > 1000) // Log slow requests (>1s)
                {
                    _logger.LogWarning(
                        "Slow request: {Method} {Path} took {Duration}ms | Status: {StatusCode}",
                        context.Request.Method,
                        context.Request.Path,
                        sw.ElapsedMilliseconds,
                        context.Response.StatusCode);
                }
                
                // Add performance header for debugging
                context.Response.Headers.Append("X-Response-Time-Ms", sw.ElapsedMilliseconds.ToString());
            }
        }
    }
}
