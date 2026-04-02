using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;
using YDeveloper.Services;

namespace YDeveloper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TicketsController : Controller
    {
        private readonly YDeveloperContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public TicketsController(YDeveloperContext context, UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index(string status = "Open")
        {
            var query = _context.Tickets.Include(t => t.Creator).AsQueryable();

            if (status != "All" && Enum.TryParse<TicketStatus>(status, out var statusEnum))
            {
                query = query.Where(t => t.Status == statusEnum);
            }

            var tickets = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
            ViewBag.CurrentStatus = status;
            return View(tickets);
        }

        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Creator)
                .Include(t => t.Messages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ticket == null) return NotFound();

            return View(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> Reply(int id, string message, string status)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Creator)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null) return NotFound();

            // Add Reply
            var reply = new TicketMessage
            {
                TicketId = id,
                SenderId = _userManager.GetUserId(User)!, // Admin ID (guaranteed non-null in authenticated context)
                Message = message,
                Timestamp = DateTime.UtcNow,
                IsInternal = false // Public reply to user
            };

            _context.TicketMessages.Add(reply);

            // Update Status
            if (Enum.TryParse<TicketStatus>(status, out var statusEnum))
            {
                ticket.Status = statusEnum;
            }

            ticket.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Notify User
            if (ticket.Creator != null && !string.IsNullOrEmpty(ticket.Creator.Email))
            {
                await _emailService.SendEmailAsync(ticket.Creator.Email, $"Destek talebiniz yanıtlandı: #{ticket.Id}",
                    $"Merhaba {ticket.Creator.FullName},<br><br>Destek talebiniz için yeni bir yanıt var:<br><em>{message}</em><br><br>Panelden detayları görüntüleyebilirsiniz.");
            }

            TempData["Success"] = "Yanıt gönderildi.";
            return RedirectToAction(nameof(Details), new { id = id });
        }
    }
}
