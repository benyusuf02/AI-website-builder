using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using YDeveloper.Constants;

namespace YDeveloper.Services
{
    /// <summary>
    /// Redis caching helper service
    /// </summary>
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;
        Task RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);
    }

    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            try
            {
                var cached = await _cache.GetStringAsync(key);
                if (string.IsNullOrEmpty(cached))
                    return null;

                return JsonSerializer.Deserialize<T>(cached);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache get error for key: {Key}", key);
                return null;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            try
            {
                var serialized = JsonSerializer.Serialize(value);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(AppConstants.DefaultCacheMinutes)
                };

                await _cache.SetStringAsync(key, serialized, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache set error for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cache remove error for key: {Key}", key);
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                var cached = await _cache.GetStringAsync(key);
                return !string.IsNullOrEmpty(cached);
            }
            catch
            {
                return false;
            }
        }
    }
}
