using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using YDeveloper.Models;

namespace YDeveloper.Controllers
{
    public class DbRepairController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public DbRepairController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpGet("fix-admin")]
        public async Task<IActionResult> FixAdmin()
        {
            var email = _configuration["Security:AdminEmail"] ?? "admin@example.com";
            var password = _configuration["Security:AdminPassword"] ?? "StrongPassword123!";

            // 1. Ensure Role Exists
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // 2. Check User
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FullName = "System Administrator (Restored)",
                    CreatedAt = DateTime.UtcNow,
                    OnboardingStep = 3
                };

                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    return Content($"SUCCESS: Admin user '{email}' created successfully! You can now log in.");
                }
                else
                {
                    return Content($"ERROR: Could not create user. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                // Ensure role
                if (!await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    return Content($"SUCCESS: User '{email}' existed but was not Admin. Role added! You can log in.");
                }

                return Content($"INFO: User '{email}' already exists and is already Admin. If you cannot login, reset password manually or delete user from DB and try again.");
            }
        }
    }
}
