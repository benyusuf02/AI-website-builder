namespace YDeveloper.Middleware
{
    public class ApiVersioningMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiVersioningMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.Headers.Append("X-API-Version", "1.0");
            }
            await _next(context);
        }
    }

    public class RequestIdMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestId = Guid.NewGuid().ToString();
            context.Items["RequestId"] = requestId;
            context.Response.Headers.Append("X-Request-ID", requestId);
            await _next(context);
        }
    }
}
