using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;

namespace YDeveloper.Controllers
{
    public class PublicController : Controller
    {
        private readonly YDeveloperContext _context;
        private readonly Services.IEmailService _emailService;

        public PublicController(YDeveloperContext context, Services.IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitForm(int siteId, IFormCollection form)
        {
            // 1. Site sahibini bul
            var site = await _context.Sites
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == siteId);

            if (site == null || site.User == null)
            {
                // Site veya sahibi bulunamadı
                return Redirect(Request.Headers["Referer"].ToString() + "?status=error");
            }

            // 2. Form verilerini topla
            var messageBody = "Web sitenizden yeni bir iletişim formu aldınız:\n\n";
            foreach (var key in form.Keys)
            {
                if (key != "siteId" && key != "__RequestVerificationToken")
                {
                    messageBody += $"{key}: {form[key]}\n";
                }
            }

            // 3. E-posta Gönderimi
            if (!string.IsNullOrEmpty(site.User.Email))
            {
                await _emailService.SendEmailAsync(site.User.Email, "Yeni İletişim Formu", messageBody);
            }

            // Log (Debug için)
            Console.WriteLine($"[MAIL SENT] To: {site.User.Email} | Body: {messageBody}");

            // 4. Geri Yönlendir (Başarılı)
            string referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer)) referer = "https://google.com"; // Fallback

            if (referer.Contains("?")) referer += "&status=success";
            else referer += "?status=success";

            return Redirect(referer);
        }
    }
}
