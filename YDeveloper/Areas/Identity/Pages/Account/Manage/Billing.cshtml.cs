using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;

namespace YDeveloper.Areas.Identity.Pages.Account.Manage
{
    public class BillingModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly YDeveloperContext _context;

        public BillingModel(UserManager<ApplicationUser> userManager, YDeveloperContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IList<PaymentTransaction> Transactions { get; set; } = new List<PaymentTransaction>();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return;
            }

            Transactions = await _context.PaymentTransactions
                .Where(t => t.UserId == user.Id)
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();
        }
    }
}
