using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;
using YDeveloper.Services;
using Microsoft.AspNetCore.Identity;

namespace YDeveloper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CouponsController : Controller
    {
        private readonly YDeveloperContext _context;
        private readonly IAuditService _auditService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CouponsController(YDeveloperContext context, IAuditService auditService, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _auditService = auditService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var coupons = await _context.Coupons.OrderByDescending(c => c.CreatedAt).ToListAsync();
            return View(coupons);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string code, int discount, DateTime expiry, int limit, string description)
        {
            if (await _context.Coupons.AnyAsync(c => c.Code == code))
            {
                TempData["Error"] = "Bu kupon kodu zaten mevcut.";
                return RedirectToAction(nameof(Index));
            }

            var coupon = new Coupon
            {
                Code = code.ToUpper(),
                DiscountPercentage = discount,
                ExpiryDate = expiry,
                MaxUses = limit,
                Description = description,
                IsActive = true
            };

            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();

            var adminId = _userManager.GetUserId(User) ?? "unknown";
            await _auditService.LogAsync(adminId, "CreateCoupon", $"Created coupon {code} ({discount}%)");

            TempData["Success"] = "Kupon oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon != null)
            {
                coupon.IsActive = !coupon.IsActive;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon != null)
            {
                _context.Coupons.Remove(coupon);
                await _context.SaveChangesAsync();

                var adminId = _userManager.GetUserId(User) ?? "unknown";
                await _auditService.LogAsync(adminId, "DeleteCoupon", $"Deleted coupon {coupon.Code}");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
