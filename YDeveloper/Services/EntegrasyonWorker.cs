using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace YDeveloper.Services
{
    public class EntegrasyonWorker : BackgroundService
    {
        private readonly ILogger<EntegrasyonWorker> _logger;

        public EntegrasyonWorker(ILogger<EntegrasyonWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EntegrasyonWorker running.");
            while (!stoppingToken.IsCancellationRequested)
            {
                // Placeholder for integration logic
                await Task.Delay(TimeSpan.FromMinutes(60), stoppingToken);
            }
        }
    }
}
