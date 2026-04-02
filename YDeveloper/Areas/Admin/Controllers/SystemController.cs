using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace YDeveloper.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SystemController : Controller
    {
        private readonly HealthCheckService _healthCheckService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public SystemController(
            HealthCheckService healthCheckService, // Built-in service
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            _healthCheckService = healthCheckService;
            _configuration = configuration;
            _env = env;
        }

        public async Task<IActionResult> Health()
        {
            var report = await _healthCheckService.CheckHealthAsync();

            // Gather other system info
            ViewBag.Environment = _env.EnvironmentName;
            ViewBag.ServerTime = DateTime.UtcNow;
            ViewBag.MachineName = Environment.MachineName;
            ViewBag.OSVersion = Environment.OSVersion.ToString();
            ViewBag.ProcessUptime = DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime();

            return View(report);
        }
    }
}
