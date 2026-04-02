namespace YDeveloper.Services
{
    public interface ISystemSettingService
    {
        Task<string> GetSettingAsync(string key, string defaultValue = "");
        Task SetSettingAsync(string key, string value);
        Task ClearCacheAsync();

        // Typed Getters for common settings
        Task<string> GetFaviconUrlAsync();
        Task<string> GetLogoUrlAsync();
        Task<string> GetSiteTitleAsync();
    }
}
