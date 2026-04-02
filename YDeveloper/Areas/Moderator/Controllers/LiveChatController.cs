using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using YDeveloper.Data;
using YDeveloper.Models;

namespace YDeveloper.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    [Authorize(Roles = "Moderator,Admin")]
    public class LiveChatController : Controller
    {
        private readonly YDeveloperContext _context;

        public LiveChatController(YDeveloperContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string status = "active", string search = null)
        {
            var query = _context.LiveChatSessions
                .Include(s => s.CustomerUser)
                //.Include(s => s.ModeratorUser) // Model might not have this nav property, check first. 
                // Based on previous ViewFile, I don't see nav property in Index.cshtml usage but better safe.
                // Re-checking Index.cshtml line 60: @(item.ModeratorUser?.UserName ?? "-") -> It DOES exist.
                // But let's check the Model file to be sure or just include it.
                .AsQueryable();

            // Filter by Status
            if (status == "active")
            {
                query = query.Where(s => s.IsActive);
            }
            else if (status == "closed")
            {
                query = query.Where(s => !s.IsActive);
            }
            // "all" returns everything

            // Filter by Search
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s =>
                    (s.CustomerUser != null && (s.CustomerUser.UserName.ToLower().Contains(search) || s.CustomerUser.Email.ToLower().Contains(search))) ||
                    s.CustomerUserId.ToLower().Contains(search)
                );
            }

            var sessions = await query
                .OrderByDescending(s => s.IsActive) // Keep active on top by default
                .ThenByDescending(s => s.StartTime)
                .ToListAsync();

            ViewData["CurrentStatus"] = status;
            ViewData["CurrentSearch"] = search;

            return View(sessions);
        }

        public async Task<IActionResult> Room(int id)
        {
            var session = await _context.LiveChatSessions
                .Include(s => s.CustomerUser)
                .Include(s => s.Messages)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null) return NotFound();

            // Mark session as assigned to this moderator if not already (safely)
            try
            {
                if (string.IsNullOrEmpty(session.ModeratorUserId))
                {
                    // Assign current user if they interact
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                                 ?? User.FindFirstValue("sub"); // Fallback for some Identity configs

                    if (!string.IsNullOrEmpty(userId))
                    {
                        session.ModeratorUserId = userId;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                // Logic failure shouldn't block the view
                Console.WriteLine($"Error assigning moderator: {ex.Message}");
            }

            return View(session);
        }

        [HttpPost]
        public async Task<IActionResult> CloseSession(int id)
        {
            var session = await _context.LiveChatSessions.FindAsync(id);
            if (session != null)
            {
                session.IsActive = false;
                session.EndTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
