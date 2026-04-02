using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;

namespace YDeveloper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AffiliatesController : Controller
    {
        private readonly YDeveloperContext _context;

        public AffiliatesController(YDeveloperContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var affiliates = await _context.Affiliates
                .Include(a => a.User)
                .OrderByDescending(a => a.TotalEarnings)
                .ToListAsync();
            return View(affiliates);
        }
    }
}
