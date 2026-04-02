using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;
using YDeveloper.Services;

namespace YDeveloper.Areas.Moderator.Controllers
{
    [Area("Moderator")]
    [Authorize(Roles = "Moderator,Admin")]
    public class SupportController : Controller
    {
        private readonly YDeveloperContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditService _auditService;
        private readonly IS3Service _s3Service;
        private readonly IConfiguration _configuration;

        public SupportController(
            YDeveloperContext context,
            UserManager<ApplicationUser> userManager,
            IAuditService auditService,
            IS3Service s3Service,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _auditService = auditService;
            _s3Service = s3Service;
            _configuration = configuration;
        }

        // --- TICKETS ---

        public async Task<IActionResult> Index(string status = "Open")
        {
            var query = _context.Tickets.Include(t => t.Creator).AsQueryable();

            if (status == "Open")
            {
                query = query.Where(t => t.Status == TicketStatus.Open || t.Status == TicketStatus.Pending);
            }
            else if (status == "Resolved")
            {
                query = query.Where(t => t.Status == TicketStatus.Resolved);
            }
            else if (status == "Escalated")
            {
                query = query.Where(t => t.Status == TicketStatus.Escalated);
            }

            var tickets = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
            ViewBag.CurrentStatus = status;
            return View(tickets);
        }

        public async Task<IActionResult> TicketDetails(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Creator)
                .Include(t => t.Messages).ThenInclude(m => m.Sender)
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null) return NotFound();

            // Get Canned Responses for dropdown
            ViewBag.CannedResponses = await _context.CannedResponses.Where(r => r.IsActive).ToListAsync();

            return View(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> ReplyTicket(int ticketId, string message, string status, IFormFile? attachment, bool internalNote = false)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null) return NotFound();

            var moderator = await _userManager.GetUserAsync(User);
            if (moderator == null) return Unauthorized();

            var ticketMessage = new TicketMessage
            {
                TicketId = ticketId,
                SenderId = moderator.Id,
                Message = message,
                Timestamp = DateTime.UtcNow,
                IsInternal = internalNote
            };

            _context.TicketMessages.Add(ticketMessage);
            await _context.SaveChangesAsync(); // Save first to get MessageId

            // Handle Attachment
            if (attachment != null && attachment.Length > 0)
            {
                var bucketName = _configuration["AWS:BucketName"];
                if (!string.IsNullOrEmpty(bucketName))
                {
                    var key = $"tickets/{ticketId}/{Guid.NewGuid()}_{attachment.FileName}";
                    using (var stream = attachment.OpenReadStream())
                    {
                        var fileUrl = await _s3Service.UploadFileAsync(bucketName, key, stream, attachment.ContentType);

                        var ticketAttachment = new TicketAttachment
                        {
                            TicketId = ticketId,
                            TicketMessageId = ticketMessage.Id,
                            FileName = attachment.FileName,
                            FileUrl = fileUrl,
                            ContentType = attachment.ContentType,
                            SizeBytes = attachment.Length
                        };
                        _context.TicketAttachments.Add(ticketAttachment);
                    }
                }
            }

            // Update Status
            if (!internalNote)
            {
                if (Enum.TryParse<TicketStatus>(status, out var newStatus))
                {
                    ticket.Status = newStatus;
                }
                ticket.UpdatedAt = DateTime.UtcNow;
                ticket.AssignedModeratorId = moderator.Id;
            }

            await _context.SaveChangesAsync();

            // Log
            if (!internalNote)
            {
                await _auditService.LogAsync(moderator.Id, "ReplyTicket", $"Replied to Ticket #{ticketId}. Status: {status}", ticket.CreatorId);
            }

            return RedirectToAction("TicketDetails", new { id = ticketId });
        }

        // --- CANNED RESPONSES ---
        // Basic CRUD for Canned Responses (Can be used by Mods or Admins)

        public async Task<IActionResult> CannedResponses()
        {
            var responses = await _context.CannedResponses.ToListAsync();
            return View(responses);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCannedResponse(string title, string content, string category)
        {
            var response = new CannedResponse
            {
                Title = title,
                Content = content,
                Category = category,
                IsActive = true
            };
            _context.CannedResponses.Add(response);
            await _context.SaveChangesAsync();
            return RedirectToAction("CannedResponses");
        }
    }
}
