using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;

namespace YDeveloper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PackagesController : Controller
    {
        private readonly YDeveloperContext _context;

        public PackagesController(YDeveloperContext context)
        {
            _context = context;
        }

        // GET: Admin/Packages
        public async Task<IActionResult> Index()
        {
            var packages = await _context.PricingPackages
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();
            
            return View(packages);
        }

        // GET: Admin/Packages/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var package = await _context.PricingPackages.FindAsync(id);
            if (package == null)
            {
                return NotFound();
            }
            
            return View(package);
        }

        // POST: Admin/Packages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PricingPackage package)
        {
            if (id != package.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update Price for backward compatibility
                    package.Price = package.MonthlyFee;
                    
                    _context.Update(package);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Paket başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PackageExists(package.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            
            return View(package);
        }

        // GET: Admin/Packages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Packages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PricingPackage package)
        {
            if (ModelState.IsValid)
            {
                package.Price = package.MonthlyFee; // Backward compatibility
                _context.PricingPackages.Add(package);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Yeni paket oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            
            return View(package);
        }

        // POST: Admin/Packages/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var package = await _context.PricingPackages.FindAsync(id);
            if (package != null)
            {
                _context.PricingPackages.Remove(package);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Paket silindi.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PackageExists(int id)
        {
            return _context.PricingPackages.Any(e => e.Id == id);
        }
    }
}
