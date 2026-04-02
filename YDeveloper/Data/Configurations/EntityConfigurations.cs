using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YDeveloper.Models;

namespace YDeveloper.Data.Configurations
{
    /// <summary>
    /// Site entity configuration - indexes, relationships
    /// </summary>
    public class SiteConfiguration : IEntityTypeConfiguration<Site>
    {
        public void Configure(EntityTypeBuilder<Site> builder)
        {
            // Indexes for performance
            builder.HasIndex(s => s.UserId).HasDatabaseName("IX_Sites_UserId");
            builder.HasIndex(s => s.Subdomain).IsUnique().HasDatabaseName("IX_Sites_Subdomain");
            builder.HasIndex(s => s.Domain).HasDatabaseName("IX_Sites_Domain");
            builder.HasIndex(s => s.IsActive).HasDatabaseName("IX_Sites_IsActive");
            builder.HasIndex(s => s.CreatedAt).HasDatabaseName("IX_Sites_CreatedAt");
        }
    }

    /// <summary>
    /// Page entity configuration
    /// </summary>
    public class PageConfiguration : IEntityTypeConfiguration<Page>
    {
        public void Configure(EntityTypeBuilder<Page> builder)
        {
            // Indexes
            builder.HasIndex(p => p.SiteId).HasDatabaseName("IX_Pages_SiteId");
            builder.HasIndex(p => p.UserId).HasDatabaseName("IX_Pages_UserId");
            builder.HasIndex(p => p.Slug).HasDatabaseName("IX_Pages_Slug");
            builder.HasIndex(p => p.IsPublished).HasDatabaseName("IX_Pages_IsPublished");
            builder.HasIndex(p => new { p.SiteId, p.Slug }).HasDatabaseName("IX_Pages_SiteId_Slug");
        }
    }

    /// <summary>
    /// PaymentTransaction entity configuration
    /// </summary>
    public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
    {
        public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
        {
            builder.HasIndex(pt => pt.UserId).HasDatabaseName("IX_PaymentTransactions_UserId");
            builder.HasIndex(pt => pt.Status).HasDatabaseName("IX_PaymentTransactions_Status");
            builder.HasIndex(pt => pt.Timestamp).HasDatabaseName("IX_PaymentTransactions_Timestamp");
            builder.HasIndex(pt => pt.TransactionId).HasDatabaseName("IX_PaymentTransactions_TransactionId");
        }
    }
}
