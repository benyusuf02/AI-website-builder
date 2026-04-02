using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;
using YDeveloper.Services;

namespace YDeveloper.Controllers.Api
{
    [Authorize]
    [Route("api/notifications")]
    [ApiController]
    public class InAppNotificationsController : ControllerBase
    {
        private readonly YDeveloperContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;

        public InAppNotificationsController(YDeveloperContext context, UserManager<ApplicationUser> userManager, INotificationService notificationService)
        {
            _context = context;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        [HttpGet("unread")]
        public async Task<IActionResult> GetUnread()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .Take(10)
                .ToListAsync();

            return Ok(notifications);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Unauthorized();

            var count = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();

            return Ok(count);
        }

        [HttpPost("read/{id}")]
        public async Task<IActionResult> MarkRead(int id)
        {
            var userId = _userManager.GetUserId(User);
            var notification = await _context.Notifications.FindAsync(id);

            if (notification == null) return NotFound();
            if (notification.UserId != userId) return Unauthorized();

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
