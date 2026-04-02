// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration; // Added
using YDeveloper.Services; // Added
using Microsoft.AspNetCore.RateLimiting;

namespace YDeveloper.Areas.Identity.Pages.Account
{
    [EnableRateLimiting("Auth")]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<YDeveloper.Models.ApplicationUser> _signInManager;
        private readonly UserManager<YDeveloper.Models.ApplicationUser> _userManager;
        private readonly IUserStore<YDeveloper.Models.ApplicationUser> _userStore;
        private readonly IUserEmailStore<YDeveloper.Models.ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IReCaptchaService _reCaptchaService;
        private readonly IConfiguration _configuration;

        public RegisterModel(
            UserManager<YDeveloper.Models.ApplicationUser> userManager,
            IUserStore<YDeveloper.Models.ApplicationUser> userStore,
            SignInManager<YDeveloper.Models.ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IReCaptchaService reCaptchaService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _reCaptchaService = reCaptchaService;
            _configuration = configuration;
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
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

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
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "Kullanıcı sözleşmesini onaylamanız gerekmektedir.")]
            [Display(Name = "Kullanıcı Sözleşmesini okudum ve kabul ediyorum.")]
            public bool AgreeToTerms { get; set; }

            public string ReCaptchaToken { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ViewData["ReCaptchaSiteKey"] = _configuration["ReCaptcha:SiteKey"];
            if (string.IsNullOrEmpty(ViewData["ReCaptchaSiteKey"] as string))
            {
                // Fallback for development if config is missing (Safety Net)
                ViewData["ReCaptchaSiteKey"] = "6LeIxAcTAAAAAJcZVRqyHh71UMIEGNQ_MXjiZKhI";
            }
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

                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                user.HasAgreedToTerms = true;
                user.TermsAgreementDate = DateTime.UtcNow;

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    var emailBody = YDeveloper.Services.EmailTemplateBuilder.Build(
                        "Hoşgeldiniz! E-postanızı Onaylayın",
                        "YDeveloper ailesine katıldığınız için teşekkür ederiz. Hesabınızı güvenli bir şekilde kullanmak için lütfen onaylayın.",
                        "Hesabımı Onayla",
                        HtmlEncoder.Default.Encode(callbackUrl)
                    );

                    await _emailSender.SendEmailAsync(Input.Email, "YDeveloper - E-posta Onayı", emailBody);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        // Add tour trigger for fresh registration
                        if (string.IsNullOrEmpty(returnUrl) || returnUrl == "/")
                        {
                            returnUrl = Url.Content("~/") + "?tour=true";
                        }
                        else
                        {
                            returnUrl += (returnUrl.Contains("?") ? "&" : "?") + "tour=true";
                        }

                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private YDeveloper.Models.ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<YDeveloper.Models.ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(YDeveloper.Models.ApplicationUser)}'. " +
                    $"Ensure that '{nameof(YDeveloper.Models.ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<YDeveloper.Models.ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<YDeveloper.Models.ApplicationUser>)_userStore;
        }
    }
}
