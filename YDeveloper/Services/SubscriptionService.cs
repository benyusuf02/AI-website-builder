using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;

namespace YDeveloper.Services
{
    public class UpgradeResult
    {
        public decimal OldPlanRemainingValue { get; set; }
        public decimal NewPlanCost { get; set; }
        public decimal AmountDue { get; set; }
        public bool IsUpgrade { get; set; } // True if paying more, False if downgrade (future credit?)
    }

    public interface ISubscriptionService
    {
        Task<UpgradeResult> CalculateProrationAsync(ApplicationUser user, int newPackageId);
        Task<bool> ProcessUpgradeAsync(ApplicationUser user, int newPackageId, string paymentTransactionId);
    }

    public class SubscriptionService : ISubscriptionService
    {
        private readonly YDeveloperContext _context;
        private readonly IPaymentService _paymentService;

        public SubscriptionService(YDeveloperContext context, IPaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        public async Task<UpgradeResult> CalculateProrationAsync(ApplicationUser user, int newPackageId)
        {
            var result = new UpgradeResult();

            // 1. Get New Package Price
            var newPackage = await _context.PricingPackages.FindAsync(newPackageId);
            if (newPackage == null) throw new Exception("Package not found");

            result.NewPlanCost = newPackage.Price;

            // 2. Calculate Old Plan Remaining Value
            if (user.CurrentPackageId.HasValue && user.PackageExpiryDate.HasValue && user.PackageExpiryDate > DateTime.UtcNow)
            {
                var currentPackage = await _context.PricingPackages.FindAsync(user.CurrentPackageId.Value);
                if (currentPackage != null)
                {
                    var totalDays = 30; // Assuming monthly. Ideally stored in user subscription start date.
                    var remainingDays = (user.PackageExpiryDate.Value - DateTime.UtcNow).TotalDays;
                    if (remainingDays < 0) remainingDays = 0;

                    var dailyRate = currentPackage.Price / totalDays;
                    result.OldPlanRemainingValue = dailyRate * (decimal)remainingDays;
                }
            }

            // 3. Calculate Due
            result.AmountDue = result.NewPlanCost - result.OldPlanRemainingValue;
            if (result.AmountDue < 0) result.AmountDue = 0; // No refunds for now, maybe store credit later

            result.IsUpgrade = result.AmountDue > 0;

            return result;
        }

        public async Task<bool> ProcessUpgradeAsync(ApplicationUser user, int newPackageId, string paymentTransactionId)
        {
            var newPackage = await _context.PricingPackages.FindAsync(newPackageId);
            if (newPackage == null) return false;

            user.CurrentPackageId = newPackage.Id;
            user.PackageExpiryDate = DateTime.UtcNow.AddDays(30); // Reset to full month from today
            user.WebsiteLimit = newPackage.WebsiteLimit;

            // Log the transaction or update success is handled by caller usually, but we update user here.
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
