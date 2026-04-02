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
    public class UsersController : Controller
    {
        private readonly YDeveloperContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditService _auditService;

        public UsersController(YDeveloperContext context, UserManager<ApplicationUser> userManager, IAuditService auditService)
        {
            _context = context;
            _userManager = userManager;
            _auditService = auditService;
        }

        // GET: /Moderator/Users
        public async Task<IActionResult> Index(string search)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => (u.Email != null && u.Email.Contains(search)) ||
                                         (u.FullName != null && u.FullName.Contains(search)) ||
                                         (u.BusinessName != null && u.BusinessName.Contains(search)));
            }

            // Fix: Return the view with the list of users
            return View(await query.ToListAsync());
        }

        // POST: /Moderator/Users/AddNote
        [HttpPost]
        public async Task<IActionResult> AddNote(string targetUserId, string noteContent)
        {
            var moderator = await _userManager.GetUserAsync(User);
            if (moderator == null) return Unauthorized();

            if (string.IsNullOrEmpty(noteContent)) return RedirectToAction("Details", new { id = targetUserId });

            var note = new UserNote
            {
                TargetUserId = targetUserId,
                AuthorId = moderator.Id,
                Note = noteContent,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserNotes.Add(note);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = targetUserId });
        }

        /* 
         * BROKEN CODE COMMENTED OUT:
         * The following logic seems to be a fragment of a ResetPassword method that was merged incorrectly into this file.
         * Logic requires 'userId' and an IdentityResult 'result' which are not defined in the context.
         * 
        // POST: /Moderator/Users/ResetPassword?
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string userId) 
        {
            var moderator = await _userManager.GetUserAsync(User);
            if (moderator == null) return Unauthorized();
            
            // Missing logic to generate token and reset password...
            // var result = ...

            /*
            if (result.Succeeded)
            {
                await _auditService.LogAsync(moderator.Id, "ResetPassword", "Manual password reset by moderator", userId);
                TempData["Success"] = "Şifre başarıyla güncellendi.";
            }
            else
            {
                TempData["Error"] = "Hata: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }
            
            return RedirectToAction("Details", new { id = userId });
        }
        */
    }
}
