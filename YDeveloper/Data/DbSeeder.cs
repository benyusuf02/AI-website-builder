using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using YDeveloper.Data;
using YDeveloper.Models;

namespace YDeveloper.Services
{
    public class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = scope.ServiceProvider.GetRequiredService<YDeveloperContext>();

            // 1. Roles
            string[] roles = { "Admin", "User", "ProUser", "Moderator" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Admin User
            var adminEmail = configuration["Security:AdminEmail"];
            var adminPassword = configuration["Security:AdminPassword"];

            if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
            {
                // No admin configuration found, skip admin creation or log warning
                // System will rely on existing users in SQL if any.
                return;
            }

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "System Administrator",
                    CreatedAt = DateTime.UtcNow,
                    OnboardingStep = 3 // Completed
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    Console.WriteLine($"[DbSeeder] Admin user created: {adminEmail}");
                }
            }

            // 3. Pricing Packages
            if (!context.PricingPackages.Any())
            {
                context.PricingPackages.AddRange(
                    new PricingPackage
                    {
                        Name = "Temel",
                        Price = 299,
                        YearlyPrice = 2990, // 2 months free
                        Description = "Bireysel girişimciler için.",
                        Features = "1 Web Sitesi, Subdomain, Temel AI Desteği",
                        IsActive = true,
                        WebsiteLimit = 1,
                        FeaturesList = new List<string> { "1 Web Sitesi", "Subdomain", "Temel AI Desteği" }
                    },
                    new PricingPackage
                    {
                        Name = "Profesyonel",
                        Price = 599,
                        YearlyPrice = 5990,
                        Description = "Büyüyen işletmeler için.",
                        Features = "1 Web Sitesi, Özel Domain, Gelişmiş AI, SEO Modülü",
                        IsActive = true,
                        IsPopular = true,
                        WebsiteLimit = 1,
                        FeaturesList = new List<string> { "1 Web Sitesi", "Özel Domain (.com.tr)", "Gelişmiş AI", "SEO Modülü" }
                    },
                    new PricingPackage
                    {
                        Name = "Premium",
                        Price = 999,
                        YearlyPrice = 9990,
                        Description = "Tam kapsamlı çözüm.",
                        Features = "1 Web Sitesi, E-Ticaret, Öncelikli Destek, Özel Tasarım",
                        IsActive = true,
                        WebsiteLimit = 1,
                        FeaturesList = new List<string> { "1 Web Sitesi", "E-Ticaret Modülü", "Öncelikli Destek", "Tam SEO Analizi" }
                    }
                );
                await context.SaveChangesAsync();
            }

            // Ensure Admin owns existing sites (if any orphan sites exist)
            var adminUserRef = await userManager.FindByEmailAsync(adminEmail);
            if (adminUserRef != null)
            {
                var orphanSites = context.Sites.Where(s => s.UserId == null || s.UserId == "").ToList();
                foreach (var site in orphanSites)
                {
                    site.UserId = adminUserRef.Id;
                }
                if (orphanSites.Any()) await context.SaveChangesAsync();
            }
        }
    }
}
