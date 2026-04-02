using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using YDeveloper.Data;
using YDeveloper.Models;

namespace YDeveloper.Services
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly YDeveloperContext _context;
        private readonly IMemoryCache _cache; // Use MemoryCache for fast synchronous-like access in Views
        private const string CacheKeyPrefix = "SystemSetting_";

        public SystemSettingService(YDeveloperContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<string> GetSettingAsync(string key, string defaultValue = "")
        {
            var cacheKey = CacheKeyPrefix + key;
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30); // Cache for 30 mins
                var setting = await _context.SystemSettings.FindAsync(key);
                return setting?.Value ?? defaultValue;
            }) ?? defaultValue;
        }

        public async Task SetSettingAsync(string key, string value)
        {
            var setting = await _context.SystemSettings.FindAsync(key);
            if (setting == null)
            {
                setting = new SystemSetting { Key = key, Value = value };
                _context.SystemSettings.Add(setting);
            }
            else
            {
                setting.Value = value;
            }
            await _context.SaveChangesAsync();
            _cache.Remove(CacheKeyPrefix + key); // Invalidate cache
        }

        public async Task ClearCacheAsync()
        {
            // MemoryCache doesn't support clearing by prefix easily without iteration, 
            // but for key specific updates we are fine. 
            // This method might be a placeholder in this simple implementation
        }

        public async Task<string> GetFaviconUrlAsync() => await GetSettingAsync("Branding:FaviconUrl", "/favicon.ico");
        public async Task<string> GetLogoUrlAsync() => await GetSettingAsync("Branding:LogoUrl", "/images/logo.png");
        public async Task<string> GetSiteTitleAsync() => await GetSettingAsync("Branding:SiteTitle", "YDeveloper");
    }
}
