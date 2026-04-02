using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;

namespace YDeveloper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SupportSettingsController : Controller
    {
        private readonly YDeveloperContext _context;

        public SupportSettingsController(YDeveloperContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.CannedResponses.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Create(string title, string content)
        {
            if (ModelState.IsValid)
            {
                _context.CannedResponses.Add(new CannedResponse { Title = title, Content = content, IsActive = true });
                await _context.SaveChangesAsync();
                TempData["Success"] = "Hazır cevap oluşturuldu.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, string title, string content)
        {
            if (ModelState.IsValid)
            {
                var item = await _context.CannedResponses.FindAsync(id);
                if (item != null)
                {
                    item.Title = title;
                    item.Content = content;
                    _context.CannedResponses.Update(item);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Güncellendi.";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.CannedResponses.FindAsync(id);
            if (item != null)
            {
                _context.CannedResponses.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
