using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using YDeveloper.Models;
using System.Threading.Tasks;

namespace YDeveloper.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly YDeveloper.Data.YDeveloperContext _context;
        private readonly YDeveloper.Services.IReCaptchaService _reCaptchaService;
        private readonly IConfiguration _configuration;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            YDeveloper.Data.YDeveloperContext context,
            YDeveloper.Services.IReCaptchaService reCaptchaService,
            IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _reCaptchaService = reCaptchaService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewBag.EnableRecaptcha = await IsRecaptchaEnabled();
            ViewBag.RecaptchaSiteKey = _configuration["Recaptcha:SiteKey"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string? recaptchaToken, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewBag.EnableRecaptcha = await IsRecaptchaEnabled();
            ViewBag.RecaptchaSiteKey = _configuration["Recaptcha:SiteKey"];

            if (await IsRecaptchaEnabled())
            {
                if (string.IsNullOrEmpty(recaptchaToken) || !await _reCaptchaService.VerifyAsync(recaptchaToken))
                {
                    ViewBag.Error = "Robot doğrulaması başarısız.";
                    return View();
                }
            }

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Email ve şifre gereklidir.";
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: true, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(email);

                // Global 2FA Enforcement Check
                if (await Is2FAEnabled() && user != null && !user.TwoFactorEnabled)
                {
                    // If 2FA is mandatory but user hasn't set it up, verify generic settings or redirect to setup
                    // For now, let's auto-enable or redirect. Ideally, redirect to a "Setup 2FA" page.
                    // But blocking them might be harsh if we haven't built the "Force Setup" flow.
                    // We will just Log it for now or rely on Identity's "RequiresTwoFactor" workflow if they had it.
                    // User request: "Pasif aktif edebilsin". If Active -> "Zorunlu". 
                    // Let's redirect to 'Manage/TwoFactorAuthentication' if they are logged in but missing it.
                    // Since they ARE logged in (result.Succeded), we can redirect them.
                    return RedirectToAction("TwoFactorAuthentication", "Manage", new { area = "Identity" });

                    // Note: If they don't have it, 'Manage' might be the best place. 
                    // But standard Identity area might be protected. They are authenticated now, so it's fine.
                }

                if (string.IsNullOrEmpty(returnUrl))
                {
                    if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                    return RedirectToAction("Index", "Dashboard");
                }
                return LocalRedirect(returnUrl);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("/Account/LoginWith2fa", new { area = "Identity", ReturnUrl = returnUrl, RememberMe = true });
            }

            if (result.IsLockedOut)
            {
                ViewBag.Error = "Hesabınız kilitlendi. Lütfen daha sonra tekrar deneyin.";
                return View();
            }

            ViewBag.Error = "Geçersiz email veya şifre.";
            return View();
        }

        private async Task<bool> IsRecaptchaEnabled()
        {
            var setting = await _context.SystemSettings.FindAsync("Security:EnableRecaptcha");
            return setting?.Value == "true";
        }

        private async Task<bool> Is2FAEnabled()
        {
            var setting = await _context.SystemSettings.FindAsync("Security:Enable2FA");
            return setting?.Value == "true";
        }



        [HttpGet]
        public async Task<IActionResult> Register()
        {
            ViewBag.EnableRecaptcha = await IsRecaptchaEnabled();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string fullName, string email, string password, string confirmPassword, string? recaptchaToken)
        {
            ViewBag.EnableRecaptcha = await IsRecaptchaEnabled();

            if (await IsRecaptchaEnabled())
            {
                if (string.IsNullOrEmpty(recaptchaToken) || !await _reCaptchaService.VerifyAsync(recaptchaToken))
                {
                    ViewBag.Error = "Robot doğrulaması başarısız.";
                    return View();
                }
            }

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Tüm alanlar zorunludur.";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Şifreler eşleşmiyor.";
                return View();
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Error = string.Join(", ", result.Errors.Select(e => e.Description));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
