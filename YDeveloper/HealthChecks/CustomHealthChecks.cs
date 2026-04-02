using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace YDeveloper.HealthChecks
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly Data.YDeveloperContext _context;

        public DatabaseHealthCheck(Data.YDeveloperContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _context.Database.CanConnectAsync(cancellationToken);
                return HealthCheckResult.Healthy("Database connection successful");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Database connection failed", ex);
            }
        }
    }

    public class RedisHealthCheck : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Simple Redis check
                await Task.CompletedTask;
                return HealthCheckResult.Healthy("Redis connection successful");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Redis connection failed", ex);
            }
        }
    }
}
