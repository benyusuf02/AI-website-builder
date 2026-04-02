using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YDeveloper.Models;

namespace YDeveloper.Data
{
    public class YDeveloperContext : IdentityDbContext<ApplicationUser>
    {
        public YDeveloperContext(DbContextOptions<YDeveloperContext> options)
            : base(options)
        {
        }

        public DbSet<Site> Sites { get; set; } = default!;
        public DbSet<Page> Pages { get; set; } = default!;
        public DbSet<PricingPackage> PricingPackages { get; set; } = default!;
        public DbSet<ContentItem> ContentItems { get; set; } = default!;
        public DbSet<Notification> Notifications { get; set; } = default!;

        // Moderator / Support System
        public DbSet<AuditLog> AuditLogs { get; set; } = default!;
        public DbSet<Ticket> Tickets { get; set; } = default!;
        public DbSet<TicketMessage> TicketMessages { get; set; } = default!;
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; } = default!;
        public DbSet<UserDevice> UserDevices { get; set; } = default!;
        public DbSet<LiveChatSession> LiveChatSessions { get; set; } = default!;
        public DbSet<LiveChatMessage> LiveChatMessages { get; set; } = default!;
        public DbSet<UserNote> UserNotes { get; set; } = default!;
        public DbSet<CannedResponse> CannedResponses { get; set; } = default!;
        public DbSet<TicketAttachment> TicketAttachments { get; set; } = default!;
        public DbSet<Coupon> Coupons { get; set; } = default!;
        public DbSet<Affiliate> Affiliates { get; set; } = default!;
        public DbSet<AffiliateClick> AffiliateClicks { get; set; } = default!;
        public DbSet<AffiliateConversion> AffiliateConversions { get; set; } = default!;
        public DbSet<CancellationRequest> CancellationRequests { get; set; } = default!;
        public DbSet<BlogPost> BlogPosts { get; set; } = default!;
        public DbSet<ContactMessage> ContactMessages { get; set; } = default!;
        public DbSet<SystemSetting> SystemSettings { get; set; } = default!;
        public DbSet<SiteAnalytics> SiteAnalytics { get; set; } = default!; // Analytics

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Fix SQL Server Multiple Cascade Paths
            builder.Entity<TicketMessage>()
                .HasOne(m => m.Ticket)
                .WithMany(t => t.Messages)
                .HasForeignKey(m => m.TicketId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cycle

            builder.Entity<TicketMessage>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cycle

            builder.Entity<Ticket>()
                .HasOne(t => t.Creator)
                .WithMany()
                .HasForeignKey(t => t.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LiveChatSession>()
                .HasOne(s => s.CustomerUser)
                .WithMany()
                .HasForeignKey(s => s.CustomerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Decimal Precision
            builder.Entity<PricingPackage>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            builder.Entity<PricingPackage>()
                .Property(p => p.YearlyPrice)
                .HasColumnType("decimal(18,2)");
        }
    }
}
