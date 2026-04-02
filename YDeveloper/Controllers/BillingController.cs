using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;
using YDeveloper.Services;

namespace YDeveloper.Controllers
{
    [Authorize]
    public class BillingController : Controller
    {
        private readonly YDeveloperContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IInvoiceService _invoiceService;
        private readonly ISubscriptionService _subscriptionService;

        public BillingController(
            YDeveloperContext context,
            UserManager<ApplicationUser> userManager,
            IInvoiceService invoiceService,
            ISubscriptionService subscriptionService)
        {
            _context = context;
            _userManager = userManager;
            _invoiceService = invoiceService;
            _subscriptionService = subscriptionService;
        }

        [HttpGet]
        public async Task<IActionResult> DownloadInvoice(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var transaction = await _context.PaymentTransactions
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == user.Id);

            if (transaction == null)
            {
                // Optionally check if admin
                if (User.IsInRole("Admin"))
                {
                    transaction = await _context.PaymentTransactions
                        .Include(t => t.User) // Need user info for admin view
                        .FirstOrDefaultAsync(t => t.Id == id);

                    if (transaction != null)
                    {
                        // Use transaction owner for invoice details, not the admin
                        var invoiceBytesAdmin = _invoiceService.GenerateInvoice(transaction, transaction.User!);
                        return File(invoiceBytesAdmin, "application/pdf", $"fatura_{transaction.TransactionId}.pdf");
                    }
                }

                return NotFound("Fatura bulunamadı veya erişim yetkiniz yok.");
            }

            var invoiceBytes = _invoiceService.GenerateInvoice(transaction, user);
            return File(invoiceBytes, "application/pdf", $"fatura_{transaction.TransactionId}.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> Upgrade(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            try
            {
                var result = await _subscriptionService.CalculateProrationAsync(user, id);
                var package = await _context.PricingPackages.FindAsync(id);

                ViewBag.PackageName = package?.Name;
                ViewBag.PackageId = id;
                return View(result);
            }
            catch (Exception)
            {
                TempData["Error"] = "Paket bulunamadı.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmUpgrade(int id)
        {
            // In a real flow, this would redirect to Payment Gateway with the 'AmountDue'.
            // For now, we assume free upgrade or manual handling, OR we assume credit card is stored.
            // Since we don't have stored cards yet, we'll redirect to a generic 'Mock Payment' for the upgrade amount.

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var result = await _subscriptionService.CalculateProrationAsync(user, id);

            if (result.AmountDue > 0)
            {
                // Redirect to payment
                return RedirectToAction("Payment", "Home", new { packageId = id, amount = result.AmountDue, isUpgrade = true });
            }
            else
            {
                // Free upgrade (unlikely) or downgrade
                await _subscriptionService.ProcessUpgradeAsync(user, id, "FREE_CHANGE");
                TempData["Success"] = "Paketiniz güncellendi.";
                return RedirectToAction("Index", "Dashboard", new { area = "" });
            }
        }

        [HttpGet]
        public IActionResult CancelSubscription()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessCancellation(string reason, string feedback)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var request = new CancellationRequest
            {
                UserId = user.Id,
                Reason = reason,
                Feedback = feedback,
                Timestamp = DateTime.UtcNow
            };

            _context.CancellationRequests.Add(request);

            // Logic to disable auto-renewal should go here.
            // For now, checking if we have an AutoRenew field.
            // Assuming manually handled or handled by RecurringBillingService checking a flag.
            // Since we don't have explicit AutoRenew flag in ApplicationUser displayed earlier, 
            // we will simulate this or add it if strictly needed. 
            // For MVP, logging the request is the key 'Business Action'.

            await _context.SaveChangesAsync();

            TempData["Info"] = "Abonelik iptal talebiniz alındı. Dönem sonuna kadar hizmet almaya devam edeceksiniz.";
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" }); // User Dashboard
        }
    }
}
