using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;

namespace YDeveloper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PaymentReportsController : Controller
    {
        private readonly YDeveloperContext _context;

        public PaymentReportsController(YDeveloperContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var transactions = await _context.PaymentTransactions
                .Include(t => t.User)
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();

            return View(transactions);
        }

        public IActionResult CreateManualTransaction()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateManualTransaction(string email, decimal amount, string note)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı.");
                return View();
            }

            var transaction = new PaymentTransaction
            {
                UserId = user.Id,
                TransactionId = $"MANUAL-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                ConversationId = $"MAN-{DateTime.UtcNow.Ticks}",
                Amount = amount,
                Currency = "TRY",
                Status = "Success",
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                IsThreeDSecure = false,
                ErrorMessage = note, // Storing note in ErrorMessage or separate field if exists. Using ErrorMessage/3DSecureInfo as "Note" field surrogate for now.
                ThreeDSecureInfo = note,
                UserAgent = "Admin Manual Entry",
                Timestamp = DateTime.UtcNow
            };

            _context.PaymentTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Manuel ödeme kaydı oluşturuldu.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.PaymentTransactions
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }
    }
}
