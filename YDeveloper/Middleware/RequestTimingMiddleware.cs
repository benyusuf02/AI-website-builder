namespace YDeveloper.Middleware
{
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTimingMiddleware> _logger;

        public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            await _next(context);
            sw.Stop();

            if (sw.ElapsedMilliseconds > 500)
            {
                _logger.LogWarning("Slow request: {Path} took {Ms}ms", 
                    context.Request.Path, sw.ElapsedMilliseconds);
            }
        }
    }
}
