using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YDeveloper.Models;
using YDeveloper.Data; // Added this using directive
using Microsoft.EntityFrameworkCore; // Kept this as it's used elsewhere (e.g., Blog, BlogPost)

namespace YDeveloper.Controllers
{
    public class HomeController : Controller
    {
        private readonly YDeveloperContext _context; // Simplified type name

        public HomeController(YDeveloperContext context) // Simplified type name
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("kullanim-kosullari")]
        public async Task<IActionResult> Terms()
        {
            var content = await _context.ContentItems.FirstOrDefaultAsync(c => c.Key == "terms_of_use");
            return View("Policy", content);
        }

        [Route("gizlilik-kvkk")]
        public async Task<IActionResult> Kvkk()
        {
            var content = await _context.ContentItems.FirstOrDefaultAsync(c => c.Key == "privacy_policy");
            return View("Policy", content);
        }

        [Route("mesafeli-satis-sozlesmesi")]
        public async Task<IActionResult> DistanceSalesAgreement()
        {
            var content = await _context.ContentItems.FirstOrDefaultAsync(c => c.Key == "distance_sales");
            return View("Policy", content);
        }

        // --- Company Pages ---
        public async Task<IActionResult> About()
        {
            var contentItems = await _context.ContentItems
                                       .Where(c => c.Section == "about")
                                       .ToDictionaryAsync(c => c.Key, c => c.Value ?? c.Title ?? c.Description); // Fallback to other fields if Value is empty

            // Should verify we have items, otherwise fallback or empty dict
            ViewBag.Content = contentItems;
            return View();
        }

        public async Task<IActionResult> Contact()
        {
            var contentItems = await _context.ContentItems
                                       .Where(c => c.Section == "contact")
                                       .ToDictionaryAsync(c => c.Key, c => c.Value ?? c.Title ?? c.Description);

            ViewBag.Content = contentItems;

            ViewData["Title"] = "İletişim";
            ViewData["Description"] = "Bize ulaşın. Sorularınız ve projeleriniz için buradayız.";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact([Bind("Name,Email,Subject,Message")] ContactMessage contactMessage)
        {
            if (ModelState.IsValid)
            {
                contactMessage.CreatedAt = DateTime.UtcNow;
                _context.Add(contactMessage);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Mesajınız başarıyla gönderildi. En kısa sürede size dönüş yapacağız.";
                return RedirectToAction(nameof(Contact));
            }
            return View(contactMessage);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                if (statusCode.Value == 404 || statusCode.Value == 400)
                {
                    ViewBag.ErrorMessage = "Aradığınız sayfa bulunamadı veya taşınmış olabilir.";
                    ViewBag.ErrorCode = statusCode.Value.ToString();
                }
                else
                {
                    ViewBag.ErrorMessage = "Beklenmedik bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.";
                    ViewBag.ErrorCode = statusCode.Value.ToString();
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Bir hata oluştu. Teknik ekibimiz bilgilendirildi.";
                ViewBag.ErrorCode = "500";
            }

            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
        [Route("Home/NotFound")]
        public IActionResult NotFoundPage()
        {
            Response.StatusCode = 404;
            return View("NotFound");
        }

    }
}
