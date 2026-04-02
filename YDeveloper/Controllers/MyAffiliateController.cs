using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;

namespace YDeveloper.Controllers
{
    [Authorize]
    public class MyAffiliateController : Controller
    {
        private readonly YDeveloperContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MyAffiliateController(YDeveloperContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var affiliate = await _context.Affiliates
                .FirstOrDefaultAsync(a => a.UserId == user.Id);

            return View(affiliate);
        }

        [HttpPost]
        public async Task<IActionResult> JoinProgram(string code)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            if (await _context.Affiliates.AnyAsync(a => a.UserId == user.Id))
            {
                return RedirectToAction(nameof(Index));
            }

            // Simple code generation if invalid
            if (string.IsNullOrWhiteSpace(code) || code.Length < 4)
            {
                code = user.UserName?.Substring(0, Math.Min(user.UserName.Length, 5)).ToUpper() + new Random().Next(100, 999);
            }
            code = code.ToUpper().Trim();

            if (await _context.Affiliates.AnyAsync(a => a.ReferralCode == code))
            {
                TempData["Error"] = "Bu kod kullanımda. Lütfen başka bir kod seçin.";
                return RedirectToAction(nameof(Index));
            }

            var affiliate = new Affiliate
            {
                UserId = user.Id,
                ReferralCode = code,
                CreatedAt = DateTime.UtcNow
            };

            _context.Affiliates.Add(affiliate);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Satış ortaklığı programına hoş geldiniz!";
            return RedirectToAction(nameof(Index));
        }
    }
}
