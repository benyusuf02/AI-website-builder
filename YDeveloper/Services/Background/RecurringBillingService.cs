using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;

namespace YDeveloper.Services.Background
{
    public class RecurringBillingService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RecurringBillingService> _logger;

        public RecurringBillingService(IServiceScopeFactory scopeFactory, ILogger<RecurringBillingService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task ProcessDailyRenewals()
        {
            _logger.LogInformation("Starting Daily Renewal Process...");

            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<YDeveloperContext>();
                var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                // 1. Find sites due for renewal
                var today = DateTime.UtcNow.Date;
                var duesSites = await context.Sites
                    .Include(s => s.User)
                    .Where(s => s.IsActive && s.NextBillingDate <= today && s.User != null && !string.IsNullOrEmpty(s.User.CardUserKey))
                    .ToListAsync();

                foreach (var site in duesSites)
                {
                    try
                    {
                        if (site.User == null || string.IsNullOrEmpty(site.User.CardUserKey)) continue;

                        _logger.LogInformation($"Attempting to charge Site {site.Id} (User: {site.User.Email})");

                        // 2. Charge
                        decimal amount = 299.00m; // Fixed price for MVP
                        bool success = await paymentService.ChargeStoredCardAsync(site.User.CardUserKey, amount);

                        if (success)
                        {
                            // 3. Success: Extend Date
                            site.NextBillingDate = (site.NextBillingDate ?? DateTime.UtcNow).AddMonths(1);
                            site.LastPaymentDate = DateTime.UtcNow;

                            // Log Payment (Optional: Add to PaymentHistory table)

                            if (!string.IsNullOrEmpty(site.User.Email))
                            {
                                await emailService.SendEmailAsync(site.User.Email, "Ödeme Başarılı", $"Sayın {site.User.FullName}, web siteniz ({site.Domain}) için aylık ödeme başarıyla alındı.");
                            }
                            _logger.LogInformation($"Site {site.Id} renewed.");
                        }
                        else
                        {
                            // 4. Fail: Notify & Suspend?
                            if (!string.IsNullOrEmpty(site.User.Email))
                            {
                                await emailService.SendEmailAsync(site.User.Email, "Ödeme Alınamadı", $"Sayın {site.User.FullName}, ödeme alınamadı. Lütfen kartınızı güncelleyin.");
                            }
                            // site.IsActive = false; // Optional suspension logic
                            _logger.LogWarning($"Site {site.Id} payment failed.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error processing site {site.Id}: {ex.Message}");
                    }
                }

                await context.SaveChangesAsync();
            }

            _logger.LogInformation("Daily Renewal Process Completed.");
        }
    }
}
