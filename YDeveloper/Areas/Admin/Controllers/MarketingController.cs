using Hangfire;
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
    public class MarketingController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IAuditService _auditService;
        private readonly YDeveloperContext _context;

        public MarketingController(
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            IAuditService auditService,
            YDeveloperContext context)
        {
            _userManager = userManager;
            _emailService = emailService;
            _auditService = auditService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userCount = await _userManager.Users.CountAsync();
            var paidCount = await _context.Sites.Where(s => s.PlanType != YDeveloper.Models.Enums.PlanType.Free).Select(s => s.UserId).Distinct().CountAsync();

            ViewBag.TotalUsers = userCount;
            ViewBag.PaidUsers = paidCount;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendBulkEmail(string subject, string body, string segment)
        {
            if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body))
            {
                TempData["Error"] = "Konu ve içerik zorunludur.";
                return RedirectToAction(nameof(Index));
            }

            // Enqueue the job
            var jobId = BackgroundJob.Enqueue(() => ProcessBulkEmail(subject, body, segment, GetAdminId()));

            await _auditService.LogAsync(GetAdminId(), "BulkEmailQueued", $"Queued email '{subject}' to segment '{segment}'. JobId: {jobId}");

            TempData["Success"] = $"E-posta gönderim işlemi sıraya alındı. (Job ID: {jobId})";
            return RedirectToAction(nameof(Index));
        }

        // Needs to be public for Hangfire, but ideally should be in a separate service
        [NonAction]
        public async Task ProcessBulkEmail(string subject, string body, string segment, string adminId)
        {
            // Note: In a real app, logic for fetching users should be inside the job execution, 
            // but controller methods can't easily be static jobs. 
            // However, Hangfire can serialize simple arguments. 
            // For complex logic, best practice is to have a dedicated service class interface (IEmailJobService).
            // For MVP, we will try to put logic here, but Controller instantiation by Hangfire might be tricky without activator.
            // BETTER: Use a dedicated method that we can call via interface. 
            // BUT: For now I will assume Hangfire can't call Controller methods directly if they depend on HttpContext.
            // So I should move this logic to a separate service or ensure this method is static/independent.
            // Wait, Hangfire activates classes using DI. So if I enqueue `x => x.Process(...)`, it works.

            IQueryable<ApplicationUser> query = _userManager.Users;

            if (segment == "Paid")
            {
                var paidUserIds = await _context.Sites
                    .Where(s => s.PlanType != YDeveloper.Models.Enums.PlanType.Free)
                    .Select(s => s.UserId)
                    .Distinct()
                    .ToListAsync();

                query = query.Where(u => paidUserIds.Contains(u.Id));
            }
            // "All" is default

            var users = await query.Select(u => new { u.Email, u.FullName }).ToListAsync();

            foreach (var user in users)
            {
                if (string.IsNullOrEmpty(user.Email)) continue;

                try
                {
                    // Customize body slightly
                    var personalizedBody = body.Replace("{Name}", user.FullName ?? "Değerli Üyemiz");
                    await _emailService.SendEmailAsync(user.Email, subject, personalizedBody);
                }
                catch
                {
                    // Continue even if one fails
                }
            }
        }

        private string GetAdminId()
        {
            return _userManager.GetUserId(User) ?? "system";
        }
    }
}
