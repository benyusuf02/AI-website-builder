using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace YDeveloper.Extensions
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                // Global limit - fallback
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.User?.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 100,
                            Window = TimeSpan.FromMinutes(1)
                        }));

                // API endpoints - stricter
                options.AddPolicy("api", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.User?.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 30,
                            Window = TimeSpan.FromMinutes(1)
                        }));

                // Auth endpoints - very strict
                options.AddPolicy("auth", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(5)
                        }));

                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = 429;
                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        Error = "Too many requests. Please try again later.",
                        RetryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter) ? retryAfter.TotalSeconds : 60
                    }, cancellationToken: token);
                };
            });

            return services;
        }
    }
}
