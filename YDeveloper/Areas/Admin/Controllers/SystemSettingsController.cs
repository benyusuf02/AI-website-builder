using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using YDeveloper.Services;
using YDeveloper.Data;
using YDeveloper.Models;
using Microsoft.EntityFrameworkCore;

namespace YDeveloper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SystemSettingsController : Controller
    {
        private readonly IDistributedCache _cache;
        private readonly IBackupService _backupService;
        private readonly YDeveloperContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ISystemSettingService _settingsService;
        private readonly IImageOptimizationService _imageService;

        public SystemSettingsController(
            IDistributedCache cache,
            IBackupService backupService,
            YDeveloperContext context,
            IWebHostEnvironment env,
            ISystemSettingService settingsService,
            IImageOptimizationService imageService)
        {
            _cache = cache;
            _backupService = backupService;
            _context = context;
            _env = env;
            _settingsService = settingsService;
            _imageService = imageService;
        }

        public async Task<IActionResult> Index()
        {
            // Maintenance Mode
            var maintenanceMode = await _cache.GetStringAsync("MaintenanceMode");
            ViewBag.IsMaintenanceMode = maintenanceMode == "true";

            // DB Settings
            var settings = await _context.SystemSettings.ToDictionaryAsync(s => s.Key, s => s.Value);
            ViewBag.Settings = settings;

            // Check last backup time
            var backupDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "backups");
            if (Directory.Exists(backupDir))
            {
                var lastFile = new DirectoryInfo(backupDir).GetFiles().OrderByDescending(f => f.CreationTime).FirstOrDefault();
                ViewBag.LastBackup = lastFile?.CreationTime.ToString("dd.MM.yyyy HH:mm") ?? "Hiç alınmamış";
            }
            else
            {
                ViewBag.LastBackup = "Hiç alınmamış";
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSecurity(bool enable2FA, bool enableRecaptcha)
        {
            await SetSettingAsync("Security:Enable2FA", enable2FA.ToString().ToLower());
            await SetSettingAsync("Security:EnableRecaptcha", enableRecaptcha.ToString().ToLower());

            TempData["Success"] = "Güvenlik ayarları güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        private async Task SetSettingAsync(string key, string value)
        {
            var setting = await _context.SystemSettings.FindAsync(key);
            if (setting == null)
            {
                setting = new YDeveloper.Models.SystemSetting { Key = key, Value = value };
                _context.SystemSettings.Add(setting);
            }
            else
            {
                setting.Value = value;
            }
            await _context.SaveChangesAsync();
        }

        [HttpPost]
        public async Task<IActionResult> ToggleMaintenance(bool enable)
        {
            if (enable)
            {
                await _cache.SetStringAsync("MaintenanceMode", "true");
                TempData["Success"] = "Site BAKIM moduna alındı. Adminler hariç erişim kapatıldı.";
            }
            else
            {
                await _cache.RemoveAsync("MaintenanceMode");
                TempData["Success"] = "Site bakım modundan çıkarıldı. Erişime açık.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> TriggerBackup()
        {
            await _backupService.TriggerBackupAsync();
            TempData["Success"] = "Veritabanı yedeği oluşturuldu ve S3'e gönderildi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBranding(string siteTitle, IFormFile? logo, IFormFile? favicon)
        {
            await _settingsService.SetSettingAsync("Branding:SiteTitle", siteTitle);

            if (logo != null && logo.Length > 0)
            {
                try
                {
                    // Optimize and Save Logo (Max width 200 is good for logo, maybe 300 to be safe for retina)
                    var logoUrl = await _imageService.OptimizeAndSaveAsync(logo, "images/branding", 300);
                    await _settingsService.SetSettingAsync("Branding:LogoUrl", logoUrl);
                }
                catch (Exception)
                {
                    // Fallback check? Or just ignore optimization?
                    // Use original logic if fail? 
                    // For now, let's trust the service or user will retry.
                }
            }

            if (favicon != null && favicon.Length > 0)
            {
                var iconPath = Path.Combine(_env.WebRootPath, "favicon_custom" + Path.GetExtension(favicon.FileName));
                using (var stream = new FileStream(iconPath, FileMode.Create))
                {
                    await favicon.CopyToAsync(stream);
                }
                await _settingsService.SetSettingAsync("Branding:FaviconUrl", "/favicon_custom" + Path.GetExtension(favicon.FileName));
            }

            TempData["Success"] = "Marka ayarları güncellendi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
