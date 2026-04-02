using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;
using YDeveloper.Services;

namespace YDeveloper.Controllers
{
    [Route("api/webhook")]
    [ApiController]
    public class WebhookController : ControllerBase
    {
        private readonly YDeveloperContext _context;
        private readonly IPaymentService _paymentService;

        public WebhookController(YDeveloperContext context, IPaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        [HttpPost("iyzico")]
        public async Task<IActionResult> IyzicoCallback([FromForm] string token)
        {
            // Iyzico callbacks usually come as POST with 'token'.
            // We need to retrieve the result using this token.

            if (string.IsNullOrEmpty(token)) return BadRequest("Token missing");

            var result = await _paymentService.RetrieveCheckoutFormResult(token);
            if (result == null) return BadRequest("Invalid token");

            if (result.Status != "success")
            {
                // Verify signature etc.
                return Ok(); // Acknowledge receipt even if failed logic internally
            }

            var conversationId = result.ConversationId;
            var paymentId = result.PaymentId;

            // Find Transaction
            var transaction = await _context.PaymentTransactions
                .FirstOrDefaultAsync(t => t.ConversationId == conversationId);

            if (transaction == null)
            {
                // Log warning: transaction not found
                return Ok();
            }

            if (transaction.Status == "Success") return Ok(); // Already processed

            if (result.PaymentStatus == "SUCCESS")
            {
                transaction.Status = "Success";
                transaction.TransactionId = paymentId;
                transaction.IsThreeDSecure = true; // Assuming checkout form is usually 3D

                // --- Affiliate Logic ---
                // We check if this user was referred by someone (Cookie logic would go here in full implementation).
                // For now, we'll check if we can link it via User -> Affiliate (Wait, Affiliate is for PROVIDING link).
                // We need to know who referred THIS user.
                // Assuming we stored 'ReferrerId' in User or a 'Cookie' -> passed to 'Metadata'.
                // Since we don't have Referrer stored in User yet, we skip this step or add TODO.
                // TODO: Implement 'Referrer' field in ApplicationUser or dedicated Tracking table.

                await _context.SaveChangesAsync();
            }
            else
            {
                transaction.Status = "Failure";
                transaction.ErrorMessage = result.ErrorMessage;
                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
