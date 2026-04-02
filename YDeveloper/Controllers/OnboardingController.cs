using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using YDeveloper.Data;
using YDeveloper.Models;
using YDeveloper.Models.Onboarding;
using YDeveloper.Services;
using System.Text.Json;

namespace YDeveloper.Controllers
{
    [Authorize]
    public class OnboardingController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly YDeveloperContext _context;
        private readonly IDomainService _domainService;
        private readonly IPaymentService _paymentService;
        private readonly GeminiService _geminiService;
        private readonly IServerInfrastructureService _serverInfrastructureService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OnboardingController> _logger;

        public OnboardingController(
            UserManager<ApplicationUser> userManager,
            YDeveloperContext context,
            IDomainService domainService,
            IPaymentService paymentService,
            GeminiService geminiService,
            IServerInfrastructureService serverInfrastructureService,
            IConfiguration configuration,
            ILogger<OnboardingController> logger)
        {
            _userManager = userManager;
            _context = context;
            _domainService = domainService;
            _paymentService = paymentService;
            _geminiService = geminiService;
            _serverInfrastructureService = serverInfrastructureService;
            _configuration = configuration;
            _logger = logger;
        }

        // Helper: Get or Create OnboardingViewModel from Session
        private OnboardingViewModel GetOnboardingData()
        {
            var json = HttpContext.Session.GetString("OnboardingData");
            if (string.IsNullOrEmpty(json))
            {
                return new OnboardingViewModel();
            }
            return JsonSerializer.Deserialize<OnboardingViewModel>(json) ?? new OnboardingViewModel();
        }

        private void SaveOnboardingData(OnboardingViewModel model)
        {
            var json = JsonSerializer.Serialize(model);
            HttpContext.Session.SetString("OnboardingData", json);
        }

        // ================== STEP 1: SITE BİLGİLERİ ==================
        [HttpGet]
        public IActionResult Step1()
        {
            var data = GetOnboardingData();
            var viewModel = new Step1ViewModel
            {
                SiteName = data.SiteName,
                SiteDescription = data.SiteDescription,
                Industry = data.Industry
            };
            ViewBag.CurrentStep = 1;
            ViewBag.TotalSteps = 5;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Step1(Step1ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CurrentStep = 1;
                ViewBag.TotalSteps = 5;
                return View(model);
            }

            var data = GetOnboardingData();
            data.SiteName = model.SiteName;
            data.SiteDescription = model.SiteDescription;
            data.Industry = model.Industry;
            data.CurrentStep = 2;
            SaveOnboardingData(data);

            return RedirectToAction("Step2");
        }

        // ================== STEP 2: TASARIM TERCİHİ ==================
        [HttpGet]
        public IActionResult Step2()
        {
            var data = GetOnboardingData();
            if (string.IsNullOrEmpty(data.SiteName))
            {
                return RedirectToAction("Step1");
            }

            var viewModel = new Step2ViewModel
            {
                ColorPalette = data.ColorPalette,
                DesignStyle = data.DesignStyle
            };
            ViewBag.CurrentStep = 2;
            ViewBag.TotalSteps = 5;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Step2(Step2ViewModel model)
        {
            var data = GetOnboardingData();
            data.ColorPalette = model.ColorPalette;
            data.DesignStyle = model.DesignStyle;
            data.CurrentStep = 3;
            SaveOnboardingData(data);

            return RedirectToAction("Step3");
        }

        // ================== STEP 3: AI İÇERİK ==================
        [HttpGet]
        public IActionResult Step3()
        {
            var data = GetOnboardingData();
            if (string.IsNullOrEmpty(data.SiteName))
            {
                return RedirectToAction("Step1");
            }

            var viewModel = new Step3ViewModel
            {
                ContentPrompt = data.ContentPrompt
            };
            ViewBag.CurrentStep = 3;
            ViewBag.TotalSteps = 5;
            ViewBag.SiteName = data.SiteName;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Step3(Step3ViewModel model)
        {
            var data = GetOnboardingData();
            data.ContentPrompt = model.ContentPrompt;
            data.CurrentStep = 4;
            SaveOnboardingData(data);

            return RedirectToAction("Step4");
        }

        // ================== STEP 4: DOMAIN & PAKET ==================
        [HttpGet]
        public IActionResult Step4()
        {
            var data = GetOnboardingData();
            if (string.IsNullOrEmpty(data.SiteName))
            {
                return RedirectToAction("Step1");
            }

            var viewModel = new Step4ViewModel
            {
                Subdomain = data.Subdomain,
                IsCustomDomain = data.IsCustomDomain,
                CustomDomain = data.CustomDomain,
                PackageType = data.PackageType
            };
            ViewBag.CurrentStep = 4;
            ViewBag.TotalSteps = 5;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Step4(Step4ViewModel model)
        {
            var data = GetOnboardingData();
            data.Subdomain = model.Subdomain;
            data.IsCustomDomain = model.IsCustomDomain;
            data.CustomDomain = model.CustomDomain;
            data.PackageType = model.PackageType;
            data.CurrentStep = 5;
            SaveOnboardingData(data);

            return RedirectToAction("Step5");
        }

        // ================== STEP 5: ÖDEME ==================
        [HttpGet]
        public IActionResult Step5()
        {
            var data = GetOnboardingData();
            if (string.IsNullOrEmpty(data.Subdomain))
            {
                return RedirectToAction("Step4");
            }

            ViewBag.CurrentStep = 5;
            ViewBag.TotalSteps = 5;
            ViewBag.OnboardingData = data;
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Step5(string paymentToken)
        {
            var data = GetOnboardingData();
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            // Ödeme işlemi simülasyonu (gerçekte Iyzico entegrasyonu)
            // TODO: Payment gateway integration

            // Site oluşturma işlemi başlasın
            return RedirectToAction("Building");
        }

        // ================== BUILDING: SİTE OLUŞTURULUYOR ==================
        [HttpGet]
        public async Task<IActionResult> Building()
        {
            var data = GetOnboardingData();
            var user = await _userManager.GetUserAsync(User);

            if (user == null || string.IsNullOrEmpty(data.SiteName))
            {
                return RedirectToAction("Step1");
            }

            ViewBag.SiteName = data.SiteName;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FinalizeCreation()
        {
            var data = GetOnboardingData();
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Json(new { success = false, message = "Oturum geçersiz" });
            }

            try
            {
                // Site oluştur
                var newSite = new Site
                {
                    UserId = user.Id,
                    Subdomain = data.Subdomain,
                    IsCustomDomain = data.IsCustomDomain,
                    Domain = data.IsCustomDomain ? data.CustomDomain : null,
                    PackageType = data.PackageType,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    SubscriptionEndDate = DateTime.UtcNow.AddMonths(1)
                };

                _context.Sites.Add(newSite);
                await _context.SaveChangesAsync();

                // AI ile içerik oluştur
                var prompt = !string.IsNullOrEmpty(data.ContentPrompt)
                    ? data.ContentPrompt
                    : $"{data.SiteName} - {data.SiteDescription} için bir {data.Industry} web sitesi oluştur";

                var generatedHtml = await _geminiService.GenerateHtmlAsync(prompt);

                // Ana sayfa oluştur
                var homePage = new Page
                {
                    SiteId = newSite.Id,
                    UserId = user.Id,
                    MetaTitle = data.SiteName,
                    Slug = "/",
                    HtmlContent = generatedHtml,
                    IsPublished = true,
                    LastPublishedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Pages.Add(homePage);
                await _context.SaveChangesAsync();

                // Session'ı temizle
                HttpContext.Session.Remove("OnboardingData");

                return Json(new { success = true, siteId = newSite.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Site oluşturma hatası");
                return Json(new { success = false, message = "Site oluşturulurken hata oluştu" });
            }
        }
    }
}
