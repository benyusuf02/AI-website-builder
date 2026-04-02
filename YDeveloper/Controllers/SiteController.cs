using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;

namespace YDeveloper.Controllers
{
    [Authorize]
    public class SiteController : Controller
    {
        private readonly YDeveloperContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SiteController(YDeveloperContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. YENİ SİTE OLUŞTURMA (GET)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User);
            // Limit Check: Max 1 Site per User
            var siteCount = await _context.Sites.CountAsync(s => s.UserId == userId);

            if (siteCount >= 1)
            {
                TempData["Error"] = "Mevcut paketinizde sadece 1 site oluşturabilirsiniz.";
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        // 2. YENİ SİTE KAYDI (POST)
        [HttpPost]
        public async Task<IActionResult> Create(
            string domain,
            string prompt,
            string? primaryColor,
            string? secondaryColor,
            string? accentColor,
            string? designStyle,
            string? targetAudience,
            string? tone)
        {
            var userId = _userManager.GetUserId(User) ?? string.Empty;

            // Double Check Limit
            var siteCount = await _context.Sites.CountAsync(s => s.UserId == userId);
            if (siteCount >= 1)
            {
                TempData["Error"] = "Mevcut paketinizde sadece 1 site oluşturabilirsiniz.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Domain temizliği
            string cleanDomain = (domain ?? "hata")
                                    .Trim()
                                    .ToLower()
                                    .Replace("https://", "")
                                    .Replace("http://", "")
                                    .Replace("www.", "")
                                    .Trim('/');

            var newSite = new YDeveloper.Models.Site
            {
                Domain = cleanDomain,
                UserId = userId,
                DesignPrompt = prompt,
                PrimaryColor = primaryColor,
                SecondaryColor = secondaryColor,
                AccentColor = accentColor,
                DesignStyle = designStyle,
                TargetAudience = targetAudience,
                Tone = tone,
                PackageType = "Giris",
                SubscriptionEndDate = DateTime.Now.AddYears(1),
                IsActive = true
            };

            _context.Sites.Add(newSite);
            await _context.SaveChangesAsync();

            // Siteden sonra Sayfa Yönetimine yönlendir (PageController)
            return RedirectToAction("Index", "Page", new { siteId = newSite.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var site = await _context.Sites.FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId);

            if (site != null)
            {
                var pages = _context.Pages.Where(p => p.SiteId == id);
                _context.Pages.RemoveRange(pages);
                _context.Sites.Remove(site);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
