using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;

namespace YDeveloper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ContentController : Controller
    {
        private readonly YDeveloperContext _context;

        public ContentController(YDeveloperContext context)
        {
            _context = context;
        }

        // GET: Admin/Content
        public async Task<IActionResult> Index(string section = "all")
        {
            var query = _context.ContentItems.AsQueryable();
            
            if (section != "all")
            {
                query = query.Where(c => c.Section == section);
            }
            
            var items = await query.OrderBy(c => c.Section).ThenBy(c => c.Key).ToListAsync();
            ViewBag.CurrentSection = section;
            
            return View(items);
        }

        // GET: Admin/Content/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _context.ContentItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            
            return View(item);
        }

        // POST: Admin/Content/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContentItem item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    item.UpdatedAt = DateTime.UtcNow;
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "İçerik başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index), new { section = item.Section });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContentItemExists(item.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            
            return View(item);
        }

        private bool ContentItemExists(int id)
        {
            return _context.ContentItems.Any(e => e.Id == id);
        }
    }
}
