using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Services;

namespace YDeveloper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly YDeveloperContext _context;
        private readonly GeminiService _geminiService;

        public ReportsController(YDeveloperContext context, GeminiService geminiService)
        {
            _context = context;
            _geminiService = geminiService;
        }

        // GET: Access Report
        public async Task<IActionResult> Access()
        {
            var last30Days = DateTime.UtcNow.AddDays(-30);

            // Real data
            var totalSites = await _context.Sites.CountAsync();
            var activeSites = await _context.Sites.CountAsync(s => s.IsActive);
            var newSitesLast30Days = await _context.Sites.CountAsync(s => s.CreatedAt >= last30Days);
            var totalPages = await _context.Pages.CountAsync();
            var publishedPages = await _context.Pages.CountAsync(p => p.IsPublished);

            // Daily site creation (last 7 days)
            var dailyStats = await _context.Sites
                .Where(s => s.CreatedAt >= DateTime.UtcNow.AddDays(-7))
                .GroupBy(s => s.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToListAsync();

            // AI Analysis
            var dataForAI = $@"
Site İstatistikleri:
- Toplam Site: {totalSites}
- Aktif Site: {activeSites}
- Son 30 Günde Yeni: {newSitesLast30Days}
- Toplam Sayfa: {totalPages}
- Yayınlanan Sayfa: {publishedPages}
- Site Aktivasyon Oranı: {(totalSites > 0 ? (double)activeSites / totalSites * 100 : 0):F1}%

Son 7 Günlük Büyüme:
{string.Join("\n", dailyStats.Select(d => $"- {d.Date:dd.MM.yyyy}: {d.Count} yeni site"))}

Bu verileri analiz et ve önemli içgörüler, trendler ve öneriler sun. Türkçe, kısa ve net ol.";

            var aiInsight = await _geminiService.GenerateContentAsync(dataForAI);

            ViewBag.TotalSites = totalSites;
            ViewBag.ActiveSites = activeSites;
            ViewBag.NewSitesLast30Days = newSitesLast30Days;
            ViewBag.TotalPages = totalPages;
            ViewBag.PublishedPages = publishedPages;
            ViewBag.DailyStats = dailyStats;
            ViewBag.AIInsight = aiInsight;

            return View();
        }

        // GET: Financial Report
        public async Task<IActionResult> Financial()
        {
            var last30Days = DateTime.UtcNow.AddDays(-30);
            var last7Days = DateTime.UtcNow.AddDays(-7);

            // Real financial data
            var totalRevenue = await _context.PaymentTransactions
                .Where(p => p.Status == "Completed")
                .SumAsync(p => p.Amount);

            var monthlyRevenue = await _context.PaymentTransactions
                .Where(p => p.Status == "Completed" && p.Timestamp >= last30Days)
                .SumAsync(p => p.Amount);

            var weeklyRevenue = await _context.PaymentTransactions
                .Where(p => p.Status == "Completed" && p.Timestamp >= last7Days)
                .SumAsync(p => p.Amount);

            var transactionCount = await _context.PaymentTransactions
                .CountAsync(p => p.Status == "Completed");

            var avgTransaction = transactionCount > 0 ? totalRevenue / transactionCount : 0;

            // Package distribution
            var packageStats = await _context.Sites
                .GroupBy(s => s.PackageType)
                .Select(g => new { Package = g.Key, Count = g.Count() })
                .ToListAsync();

            // Daily revenue (last 7 days)
            var dailyRevenue = await _context.PaymentTransactions
                .Where(p => p.Status == "Completed" && p.Timestamp >= last7Days)
                .GroupBy(p => p.Timestamp.Date)
                .Select(g => new { Date = g.Key, Amount = g.Sum(x => x.Amount) })
                .OrderBy(x => x.Date)
                .ToListAsync();

            // AI Analysis
            var dataForAI = $@"
Finansal Özet:
- Toplam Gelir: ₺{totalRevenue:N2}
- Aylık Gelir (30 gün): ₺{monthlyRevenue:N2}
- Haftalık Gelir (7 gün): ₺{weeklyRevenue:N2}
- Toplam İşlem: {transactionCount}
- Ortalama İşlem: ₺{avgTransaction:N2}

Paket Dağılımı:
{string.Join("\n", packageStats.Select(p => $"- {p.Package}: {p.Count} kullanıcı"))}

Son 7 Günlük Gelir:
{string.Join("\n", dailyRevenue.Select(d => $"- {d.Date:dd.MM.yyyy}: ₺{d.Amount:N2}"))}

Bu finansal verileri analiz et, trendleri değerlendir ve gelir artırma önerileri sun. Türkçe, profesyonel ve aksiyon odaklı ol.";

            var aiInsight = await _geminiService.GenerateContentAsync(dataForAI);

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.MonthlyRevenue = monthlyRevenue;
            ViewBag.WeeklyRevenue = weeklyRevenue;
            ViewBag.TransactionCount = transactionCount;
            ViewBag.AvgTransaction = avgTransaction;
            ViewBag.PackageStats = packageStats;
            ViewBag.DailyRevenue = dailyRevenue;
            ViewBag.AIInsight = aiInsight;

            return View();
        }

        // GET: User Report
        public async Task<IActionResult> Users()
        {
            var last30Days = DateTime.UtcNow.AddDays(-30);
            var last7Days = DateTime.UtcNow.AddDays(-7);

            // Real user data
            var totalUsers = await _context.Users.CountAsync();
            var newUsersLast30Days = await _context.Users.CountAsync(u => u.CreatedAt >= last30Days);
            var newUsersLast7Days = await _context.Users.CountAsync(u => u.CreatedAt >= last7Days);
            var usersWithSites = await _context.Users.CountAsync(u => _context.Sites.Any(s => s.UserId == u.Id));
            var usersWithoutSites = totalUsers - usersWithSites;

            // Daily user registration (last 7 days)
            var dailyRegistrations = await _context.Users
                .Where(u => u.CreatedAt >= last7Days)
                .GroupBy(u => u.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToListAsync();

            // Active users (users with sites)
            var conversionRate = totalUsers > 0 ? (double)usersWithSites / totalUsers * 100 : 0;

            // AI Analysis
            var dataForAI = $@"
Kullanıcı İstatistikleri:
- Toplam Kullanıcı: {totalUsers}
- Son 30 Günde Yeni: {newUsersLast30Days}
- Son 7 Günde Yeni: {newUsersLast7Days}
- Sitesi Olan: {usersWithSites}
- Sitesi Olmayan: {usersWithoutSites}
- Dönüşüm Oranı: {conversionRate:F1}% (sitesi olan / toplam)

Son 7 Günlük Kayıtlar:
{string.Join("\n", dailyRegistrations.Select(d => $"- {d.Date:dd.MM.yyyy}: {d.Count} yeni kullanıcı"))}

Bu kullanıcı verilerini analiz et, engagement oranlarını değerlendir ve kullanıcı aktivasyonu için öneriler sun. Türkçe, aksiyon odaklı ol.";

            var aiInsight = await _geminiService.GenerateContentAsync(dataForAI);

            ViewBag.TotalUsers = totalUsers;
            ViewBag.NewUsersLast30Days = newUsersLast30Days;
            ViewBag.NewUsersLast7Days = newUsersLast7Days;
            ViewBag.UsersWithSites = usersWithSites;
            ViewBag.UsersWithoutSites = usersWithoutSites;
            ViewBag.ConversionRate = conversionRate;
            ViewBag.DailyRegistrations = dailyRegistrations;
            ViewBag.AIInsight = aiInsight;

            return View();
        }
    }
}
