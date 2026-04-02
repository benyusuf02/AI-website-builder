using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;

namespace YDeveloper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PricingController : Controller
    {
        private readonly YDeveloperContext _context;

        public PricingController(YDeveloperContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var packages = await _context.PricingPackages
                .OrderBy(p => p.Price)
                .ToListAsync();
            return View(packages);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PricingPackage package)
        {
            if (ModelState.IsValid)
            {
                _context.Add(package);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Paket başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            return View(package);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var package = await _context.PricingPackages.FindAsync(id);
            if (package == null) return NotFound();

            return View(package);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PricingPackage package)
        {
            if (id != package.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(package);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Paket güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PackageExists(package.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(package);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var package = await _context.PricingPackages.FindAsync(id);
            if (package != null)
            {
                _context.PricingPackages.Remove(package);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Paket silindi.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var package = await _context.PricingPackages.FindAsync(id);
            if (package != null)
            {
                package.IsActive = !package.IsActive;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Paket durumu {(package.IsActive ? "Aktif" : "Pasif")} olarak güncellendi.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PackageExists(int id)
        {
            return _context.PricingPackages.Any(e => e.Id == id);
        }
    }
}
