using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;

namespace YDeveloper.Areas.Identity.Pages.Account.Manage
{
    public class SessionsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly YDeveloperContext _context;

        // Initialize optional properties
        public SessionsModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            YDeveloperContext context,
            Microsoft.Extensions.Logging.ILogger<SessionsModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
            UserDevices = new List<UserDevice>();
        }

        public IList<UserDevice> UserDevices { get; set; }

        [TempData]
        public string? StatusMessage { get; set; }

        private readonly Microsoft.Extensions.Logging.ILogger<SessionsModel> _logger;

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            UserDevices = await _context.UserDevices
                .Where(d => d.UserId == user.Id)
                .OrderByDescending(d => d.LastActive)
                .Take(20) // Show last 20 sessions
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostSignOutAllAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Update Security Stamp to invalidate all other tokens
            await _userManager.UpdateSecurityStampAsync(user);

            // Optional: Clear database records logic if desired, or keep logic log
            // await _context.UserDevices.Where(d => d.UserId == user.Id).ExecuteDeleteAsync();

            await _signInManager.SignOutAsync();

            _logger.LogInformation("User logged out from all devices.");
            StatusMessage = "Başarıyla tüm cihazlardan çıkış yapıldı. Lütfen tekrar giriş yapın.";

            return RedirectToPage("../Login");
        }
    }
}
