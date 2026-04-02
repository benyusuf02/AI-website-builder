using YDeveloper.Models;

namespace YDeveloper.Services
{
    public interface IPageService
    {
        Task<Page?> GetPageAsync(int pageId, string userId);
        Task<Page> CreatePageAsync(int siteId, string userId, string pageName, string slug, string prompt);
        Task<bool> UpdateContentAsync(int pageId, string userId, string htmlContent);
        Task<bool> DeletePageAsync(int pageId, string userId);
        Task<string> PublishPageAsync(int pageId, string userId, string? htmlContent = null);
        Task SyncMenusAsync(int siteId);
    }
}
