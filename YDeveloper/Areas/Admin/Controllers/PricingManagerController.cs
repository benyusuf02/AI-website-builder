using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;

namespace YDeveloper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PricingManagerController : Controller
    {
        private readonly YDeveloperContext _context;

        public PricingManagerController(YDeveloperContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.PricingPackages.OrderBy(p => p.Price).ToListAsync());
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var pricingPackage = await _context.PricingPackages.FindAsync(id);
            if (pricingPackage == null) return NotFound();
            return View(pricingPackage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PricingPackage pricingPackage)
        {
            if (id != pricingPackage.Id) return NotFound();

            // Fetch existing to prevent overwriting missing form fields (like Description) with null
            var existingPackage = await _context.PricingPackages.FindAsync(id);
            if (existingPackage == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Update only allowed fields
                    existingPackage.Name = pricingPackage.Name;
                    existingPackage.Price = pricingPackage.Price;
                    existingPackage.BillingPeriod = pricingPackage.BillingPeriod;
                    existingPackage.ButtonText = pricingPackage.ButtonText;
                    existingPackage.WebsiteLimit = pricingPackage.WebsiteLimit;
                    existingPackage.IsPopular = pricingPackage.IsPopular;
                    existingPackage.IsActive = pricingPackage.IsActive;

                    // Update Description and Features
                    existingPackage.Description = pricingPackage.Description;
                    existingPackage.Features = pricingPackage.Features;

                    // Keep YearlyPrice intact if not in form
                    // If you add them to the form later, update them here.

                    _context.Update(existingPackage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PricingPackageExists(pricingPackage.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pricingPackage);
        }

        private bool PricingPackageExists(int id)
        {
            return _context.PricingPackages.Any(e => e.Id == id);
        }
    }
}
