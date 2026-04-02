// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration; // Added
using YDeveloper.Services; // Added
using Microsoft.AspNetCore.RateLimiting;

namespace YDeveloper.Areas.Identity.Pages.Account
{
    [EnableRateLimiting("Auth")]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<YDeveloper.Models.ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IReCaptchaService _reCaptchaService;
        private readonly IConfiguration _configuration;
        private readonly YDeveloper.Data.YDeveloperContext _context;

        public LoginModel(SignInManager<YDeveloper.Models.ApplicationUser> signInManager, ILogger<LoginModel> logger, IReCaptchaService reCaptchaService, IConfiguration configuration, YDeveloper.Data.YDeveloperContext context)
        {
            _signInManager = signInManager;
            _logger = logger;
            _reCaptchaService = reCaptchaService;
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }

            public string ReCaptchaToken { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
            ViewData["ReCaptchaSiteKey"] = _configuration["ReCaptcha:SiteKey"];
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // Verify ReCaptcha
                if (!await _reCaptchaService.VerifyAsync(Input.ReCaptchaToken))
                {
                    ModelState.AddModelError(string.Empty, "Google ReCaptcha doğrulaması başarısız. Lütfen tekrar deneyin.");
                    ViewData["ReCaptchaSiteKey"] = _configuration["ReCaptcha:SiteKey"];
                    return Page();
                }

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    // Track Device
                    var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);
                    if (user != null)
                    {
                        // Update Last Login
                        user.LastLoginDate = DateTime.UtcNow;

                        var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
                        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                        // Simple parsing
                        var deviceName = "Unknown Device";
                        if (userAgent.Contains("Windows")) deviceName = "Windows Device";
                        else if (userAgent.Contains("Mac")) deviceName = "Mac Device";
                        else if (userAgent.Contains("Linux")) deviceName = "Linux Device";
                        else if (userAgent.Contains("Android")) deviceName = "Android Device";
                        else if (userAgent.Contains("iPhone") || userAgent.Contains("iPad")) deviceName = "iOS Device";

                        if (userAgent.Contains("Chrome")) deviceName += " (Chrome)";
                        else if (userAgent.Contains("Firefox")) deviceName += " (Firefox)";
                        else if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome")) deviceName += " (Safari)";
                        else if (userAgent.Contains("Edge")) deviceName += " (Edge)";

                        var device = new YDeveloper.Models.UserDevice
                        {
                            UserId = user.Id,
                            UserAgent = userAgent,
                            IpAddress = ipAddress,
                            DeviceName = deviceName,
                            LastActive = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow
                        };

                        _context.UserDevices.Add(device);
                        await _context.SaveChangesAsync();
                    }

                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
