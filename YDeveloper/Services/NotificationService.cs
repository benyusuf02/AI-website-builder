using System.Threading.Tasks;
using YDeveloper.Data;
using YDeveloper.Models;

namespace YDeveloper.Services
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(string userId, string title, string message, string? actionUrl = null);
        Task MarkAsReadAsync(int notificationId);
    }

    public class NotificationService : INotificationService
    {
        private readonly YDeveloperContext _context;

        public NotificationService(YDeveloperContext context)
        {
            _context = context;
        }

        public async Task CreateNotificationAsync(string userId, string title, string message, string? actionUrl = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                ActionUrl = actionUrl,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
