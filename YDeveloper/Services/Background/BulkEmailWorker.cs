using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Data;
using YDeveloper.Models;

namespace YDeveloper.Services.Background
{
    public interface IBulkEmailWorker
    {
        Task ProcessBulkEmailAsync(string subject, string body, string segment, string adminId);
    }

    public class BulkEmailWorker : IBulkEmailWorker
    {
        private readonly IServiceProvider _serviceProvider;
        // We use IServiceProvider to create a scope because background jobs run outside of request scope
        // and DbContext is scoped.

        public BulkEmailWorker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ProcessBulkEmailAsync(string subject, string body, string segment, string adminId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var context = scope.ServiceProvider.GetRequiredService<YDeveloperContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<BulkEmailWorker>>();

                logger.LogInformation($"Starting bulk email job. Subject: {subject}, Segment: {segment}");

                IQueryable<ApplicationUser> query = userManager.Users;

                if (segment == "Paid")
                {
                    var paidUserIds = await context.Sites
                        .Where(s => s.PlanType != YDeveloper.Models.Enums.PlanType.Free)
                        .Select(s => s.UserId)
                        .Distinct()
                        .ToListAsync();

                    query = query.Where(u => paidUserIds.Contains(u.Id));
                }
                // "All" is default

                var users = await query.Select(u => new { u.Email, u.FullName }).ToListAsync();
                logger.LogInformation($"Found {users.Count} users to email.");

                int success = 0;
                int fail = 0;

                foreach (var user in users)
                {
                    if (string.IsNullOrEmpty(user.Email)) continue;

                    try
                    {
                        var personalizedBody = body.Replace("{Name}", user.FullName ?? "Değerli Üyemiz");
                        await emailService.SendEmailAsync(user.Email, subject, personalizedBody);
                        success++;
                    }
                    catch (Exception ex)
                    {
                        fail++;
                        logger.LogError(ex, $"Failed to send email to {user.Email}");
                    }
                }

                logger.LogInformation($"Bulk email finished. Success: {success}, Fail: {fail}");
            }
        }
    }
}
