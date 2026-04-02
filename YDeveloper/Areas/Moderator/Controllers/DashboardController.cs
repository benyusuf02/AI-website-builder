using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;

namespace YDeveloper.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    [Authorize(Roles = "Moderator,Admin")] // Admin can also access
    public class DashboardController : Controller
    {
        private readonly YDeveloperContext _context;

        public DashboardController(YDeveloperContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // --- ANALYTICS ---
            var today = DateTime.UtcNow.Date;

            // 1. Pending Tickets
            var pendingTickets = await _context.Tickets
                .Where(t => t.Status == Models.TicketStatus.Open || t.Status == Models.TicketStatus.Pending)
                .CountAsync();

            // 2. Active Chats
            var activeChats = await _context.LiveChatSessions
                .Where(c => c.IsActive)
                .CountAsync();

            // 3. Tickets Resolved Today
            var resolvedToday = await _context.Tickets
                .Where(t => t.Status == Models.TicketStatus.Resolved && t.UpdatedAt >= today)
                .CountAsync();

            // 4. Recently Active Users (Login in last 24h - approximate if we had LastLogin field, using Tickets for now)
            var recentActivity = await _context.Tickets
                .OrderByDescending(t => t.CreatedAt)
                .Take(5)
                .Include(t => t.Creator)
                .ToListAsync();

            ViewBag.PendingTickets = pendingTickets;
            ViewBag.ActiveChats = activeChats;
            ViewBag.ResolvedToday = resolvedToday;

            return View(recentActivity);
        }
    }
}
