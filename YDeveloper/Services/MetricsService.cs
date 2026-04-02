namespace YDeveloper.Services
{
    public interface IMetricsService
    {
        Task IncrementCounterAsync(string metricName);
        Task RecordValueAsync(string metricName, double value);
        Task<Dictionary<string, long>> GetMetricsAsync();
    }

    public class MetricsService : IMetricsService
    {
        private readonly Services.ICacheService _cache;

        public MetricsService(ICacheService cache)
        {
            _cache = cache;
        }

        public async Task IncrementCounterAsync(string metricName)
        {
            var key = $"metrics:{metricName}";
            var current = await _cache.GetAsync<string>(key);
            var value = string.IsNullOrEmpty(current) ? 0 : long.Parse(current);
            await _cache.SetAsync(key, (value + 1).ToString(), TimeSpan.FromDays(30));
        }

        public async Task RecordValueAsync(string metricName, double value)
        {
            var key = $"metrics:{metricName}:values";
            var values = await _cache.GetAsync<List<double>>(key) ?? new List<double>();
            values.Add(value);
            if (values.Count > 1000) values.RemoveAt(0); // Keep last 1000
            await _cache.SetAsync(key, values, TimeSpan.FromDays(30));
        }

        public Task<Dictionary<string, long>> GetMetricsAsync()
        {
            return Task.FromResult(new Dictionary<string, long>()); // Stub implementation
        }
    }
}
