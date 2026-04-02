using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using YDeveloper.Models;
using YDeveloper.Models.ViewModels;
using System.Threading.Tasks;

namespace YDeveloper.Controllers
{
    public class LegalController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public LegalController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        private async Task<LegalDocumentViewModel> GetLegalModelAsync(string title, string contractId)
        {
            var model = new LegalDocumentViewModel
            {
                Title = title,
                ContractId = contractId,
                GeneratedDate = DateTime.Now,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0"
            };

            if (User?.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    model.FullName = user.FullName ?? user.UserName ?? "Kullanıcı";
                    model.Email = user.Email ?? "-";
                    model.BusinessName = user.BusinessName ?? "Bireysel";

                    // Simple address composition
                    var parts = new[] { user.Address, user.City, user.Country, user.ZipCode };
                    var fullAddress = string.Join(", ", parts.Where(x => !string.IsNullOrEmpty(x)));
                    model.Address = !string.IsNullOrEmpty(fullAddress) ? fullAddress : "Adres Girilmemiş";
                }
            }

            return model;
        }

        public async Task<IActionResult> Terms()
        {
            var model = await GetLegalModelAsync("Kullanım Koşulları", "TOS-2025-V1");
            return View(model);
        }

        public async Task<IActionResult> Privacy()
        {
            var model = await GetLegalModelAsync("Gizlilik Politikası", "PP-2025-V1");
            return View(model);
        }

        public async Task<IActionResult> DistancedSales()
        {
            var model = await GetLegalModelAsync("Mesafeli Satış Sözleşmesi", "DSS-2025-V1");
            return View(model);
        }
    }
}
