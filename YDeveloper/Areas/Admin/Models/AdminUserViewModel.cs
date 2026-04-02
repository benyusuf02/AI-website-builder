using System;
using YDeveloper.Models;

namespace YDeveloper.Areas.Admin.Models
{
    public class AdminUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsBanned { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public int SiteCount { get; set; }
        public int WebsiteLimit { get; set; }
        public string? AdminNote { get; set; }
    }

    public class AdminUserDetailViewModel
    {
        public ApplicationUser User { get; set; } = default!;
        public List<Site> Sites { get; set; } = new();
        public List<PaymentTransaction> Transactions { get; set; } = new();
        public List<AuditLog> AuditLogs { get; set; } = new();
        public List<Ticket> Tickets { get; set; } = new();
        public List<string> Roles { get; set; } = new();
    }
}
