namespace YDeveloper.Middleware
{
    /// <summary>
    /// Security headers middleware - CSP, X-Frame-Options, etc.
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Content Security Policy
            context.Response.Headers.Append("Content-Security-Policy",
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net https://cdn.jsdelivr.net https://unpkg.com https://www.google.com https://www.gstatic.com; " +
                "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://fonts.googleapis.com; " +
                "font-src 'self' https://cdn.jsdelivr.net https://fonts.gstatic.com; " +
                "img-src 'self' data: https: blob:; " +
                "connect-src 'self' https://generativelanguage.googleapis.com; " +
                "frame-ancestors 'self'; " +
                "base-uri 'self'; " +
                "form-action 'self';");

            // Security headers (already in Program.cs but centralized here)
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
            context.Response.Headers.Append("Permissions-Policy", "geolocation=(), microphone=(), camera=()");

            // Remove server header
            context.Response.Headers.Remove("Server");
            context.Response.Headers.Remove("X-Powered-By");

            await _next(context);
        }
    }
}
