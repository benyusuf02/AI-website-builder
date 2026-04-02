using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;

namespace YDeveloper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly YDeveloperContext _context;

        public DashboardController(YDeveloperContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalSites = await _context.Sites.CountAsync();
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.MonthlyRevenue = await _context.PaymentTransactions
                .Where(p => p.Timestamp >= DateTime.UtcNow.AddMonths(-1) && p.Status == "Completed")
                .SumAsync(p => p.Amount);
            ViewBag.UnreadMessages = await _context.ContactMessages.CountAsync(m => !m.IsRead);

            return View();
        }
    }
}
