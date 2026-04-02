using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;

namespace YDeveloper.Controllers
{
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class DashboardController : Controller
    {
        private readonly YDeveloperContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(YDeveloperContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. MODERN DASHBOARD - Analytics ile
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Login", "Account", new { area = "Identity" });

            // Kullanıcının sitelerini getir
            var sites = await _context.Sites
                .Where(s => s.UserId == userId)
                .Include(s => s.Pages)
                .ToListAsync();

            // İlk siteyi seçili yap
            var selectedSite = sites.FirstOrDefault();

            if (selectedSite == null)
            {
                // Henüz site oluşturulmamış - onboarding'e yönlendir
                ViewBag.NoSite = true;
                return View(new DashboardViewModel());
            }

            // Analytics verilerini mock olarak hazırla (gerçek analytics implementasyonu sonra eklenebilir)
            var today = DateTime.UtcNow.Date;
            var monthStart = new DateTime(today.Year, today.Month, 1);

            // Mock analytics data
            var random = new Random();
            var todayVisitors = random.Next(50, 200);
            var monthVisitors = random.Next(500, 2000);

            // Last 7 days mock graph data
            var last7Days = Enumerable.Range(0, 7).Select(i =>
            {
                var date = today.AddDays(-6 + i);
                return new ChartDataPoint
                {
                    Label = date.ToString("dd MMM"),
                    Value = random.Next(20, 150)
                };
            }).ToList();

            var viewModel = new DashboardViewModel
            {
                SiteName = selectedSite.Subdomain ?? selectedSite.Domain ?? "Site",
                Domain = selectedSite.IsCustomDomain ? selectedSite.Domain : $"{selectedSite.Subdomain}.ydeveloper.com",
                IsActive = selectedSite.IsActive,
                TotalPages = selectedSite.Pages?.Count ?? 0,
                TodayVisitors = todayVisitors,
                MonthVisitors = monthVisitors,
                LastUpdated = selectedSite.Pages?.OrderByDescending(p => p.LastPublishedAt).FirstOrDefault()?.LastPublishedAt,
                VisitorChartData = last7Days,
                TopPages = new Dictionary<string, int>
                {
                    { "Ana Sayfa", random.Next(100, 300) },
                    { "Hakkımızda", random.Next(50, 150) },
                    { "İletişim", random.Next(30, 100) }
                }
            };

            ViewBag.Sites = sites;
            ViewBag.SelectedSiteId = selectedSite.Id;

            return View(viewModel);
        }

        // 2. ANALYTICS SAYFASI
        public async Task<IActionResult> Analytics(int siteId)
        {
            var userId = _userManager.GetUserId(User);
            var site = await _context.Sites
                .Include(s => s.Pages)
                .FirstOrDefaultAsync(s => s.Id == siteId && s.UserId == userId);

            if (site == null) return NotFound();

            ViewBag.SiteName = site.Subdomain ?? site.Domain;
            ViewBag.SiteId = siteId;

            return View();
        }

        // 3. SETTINGS SAYFASI
        public async Task<IActionResult> Settings(int siteId)
        {
            var userId = _userManager.GetUserId(User);
            var site = await _context.Sites.FirstOrDefaultAsync(s => s.Id == siteId && s.UserId == userId);

            if (site == null) return NotFound();

            return View(site);
        }
    }
}