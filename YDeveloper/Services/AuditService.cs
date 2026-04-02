using YDeveloper.Data;
using YDeveloper.Models;

namespace YDeveloper.Services
{
    public interface IAuditService
    {
        Task LogAsync(string performerId, string action, string details, string? targetUserId = null, string? ipAddress = null);
    }

    public class AuditService : IAuditService
    {
        private readonly YDeveloperContext _context;

        public AuditService(YDeveloperContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string performerId, string action, string details, string? targetUserId = null, string? ipAddress = null)
        {
            var log = new AuditLog
            {
                PerformerUserId = performerId,
                Action = action,
                Details = details,
                TargetUserId = targetUserId ?? "System",
                IpAddress = ipAddress ?? "Unknown",
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
