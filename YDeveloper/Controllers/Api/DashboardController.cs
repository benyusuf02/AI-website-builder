using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace YDeveloper.Controllers.Api
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Secure endpoint
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly YDeveloperContext _context;

        public DashboardController(YDeveloperContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetStats()
        {
            var today = DateTime.UtcNow.Date;

            var totalUsers = await _context.Users.CountAsync();

            // Use Sites as proxy for Active Subscriptions
            var activeSubs = await _context.Sites.CountAsync(s => s.IsActive && s.SubscriptionEndDate > DateTime.UtcNow);

            // Join Sites with Packages to calculate revenue
            var monthlyRevenue = await _context.Sites
                .Where(s => s.IsActive && s.SubscriptionEndDate > DateTime.UtcNow)
                .Join(_context.PricingPackages, site => site.PackageType, pkg => pkg.Name, (site, pkg) => pkg.Price)
                .SumAsync();

            var newUsersToday = await _context.Users
                .CountAsync(u => u.CreatedAt >= today);

            // Corrected DbSet name: Tickets
            var warningCount = await _context.Tickets
                .CountAsync(t => t.Status == YDeveloper.Models.TicketStatus.Open);

            var stats = new DashboardStatsDto
            {
                TotalUsers = totalUsers,
                ActiveSubscriptions = activeSubs,
                EstimatedMonthlyRevenue = monthlyRevenue,
                NewUsersToday = newUsersToday,
                WarningCount = warningCount
            };

            return Ok(stats);
        }
    }
}
