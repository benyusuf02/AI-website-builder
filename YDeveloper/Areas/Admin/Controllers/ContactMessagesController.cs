using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;

namespace YDeveloper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ContactMessagesController : Controller
    {
        private readonly YDeveloperContext _context;

        public ContactMessagesController(YDeveloperContext context)
        {
            _context = context;
        }

        // GET: Admin/ContactMessages
        public async Task<IActionResult> Index(bool? unreadOnly)
        {
            var query = _context.ContactMessages.AsQueryable();
            
            if (unreadOnly == true)
            {
                query = query.Where(m => !m.IsRead);
            }
            
            var messages = await query
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
                
            ViewBag.UnreadOnly = unreadOnly ?? false;
            ViewBag.UnreadCount = await _context.ContactMessages.CountAsync(m => !m.IsRead);
            
            return View(messages);
        }

        // GET: Admin/ContactMessages/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            // Mark as read
            if (!message.IsRead)
            {
                message.IsRead = true;
                _context.Update(message);
                await _context.SaveChangesAsync();
            }

            return View(message);
        }

        // POST: Admin/ContactMessages/AddNote/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNote(int id, string notes)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            message.AdminNotes = notes;
            _context.Update(message);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Not eklendi.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Admin/ContactMessages/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var message = await _context.ContactMessages.FindAsync(id);
            if (message != null)
            {
                _context.ContactMessages.Remove(message);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Mesaj silindi.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
