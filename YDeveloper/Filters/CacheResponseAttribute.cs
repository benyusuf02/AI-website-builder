using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Cryptography;
using System.Text;

namespace YDeveloper.Filters
{
    /// <summary>
    /// Response caching attribute for controller actions
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CacheResponseAttribute : ActionFilterAttribute
    {
        private readonly int _duration;

        public CacheResponseAttribute(int durationSeconds = 60)
        {
            _duration = durationSeconds;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cache = context.HttpContext.RequestServices.GetRequiredService<IDistributedCache>();
            var cacheKey = GenerateCacheKey(context.HttpContext.Request);

            // Try get from cache
            var cachedResponse = await cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedResponse))
            {
                context.Result = new ContentResult
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                return;
            }

            // Execute action
            var executedContext = await next();

            // Cache the result
            if (executedContext.Result is ObjectResult objectResult && objectResult.Value != null)
            {
                var serialized = System.Text.Json.JsonSerializer.Serialize(objectResult.Value);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_duration)
                };
                await cache.SetStringAsync(cacheKey, serialized, options);
            }
        }

        private string GenerateCacheKey(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"{request.Path}");
            
            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}={value}");
            }

            return ComputeHash(keyBuilder.ToString());
        }

        private string ComputeHash(string input)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }
    }
}
