using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace YDeveloper.Models
{
    public class Site
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser? User { get; set; }

        // Domain Info
        public string? Domain { get; set; } // Custom domain (e.g. myshop.com)
        public string Subdomain { get; set; } = string.Empty; // Subdomain (e.g. myshop.ydeveloper.com)
        public bool IsCustomDomain { get; set; } = false;

        // Hosting Info
        public bool IsPublished { get; set; } = false;
        public string? S3Url { get; set; } // URL to S3 bucket index.html

        // Subscription
        public YDeveloper.Models.Enums.PlanType PlanType { get; set; } = Enums.PlanType.Free;
        public DateTime? NextBillingDate { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public string? PaymentCardUserKey { get; set; } // Iyzico Card User Key for Recurring
        public string? PaymentCardToken { get; set; }   // Iyzico Card Token for Recurring

        // AI & Design
        public string PackageType { get; set; } = "Basic"; // Legacy field, keeping for compatibility
        public DateTime SubscriptionEndDate { get; set; }
        public string DesignPrompt { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Added for Roadmap Item 28 tracking

        // Tasarım Tercihleri (Yeni Eklenenler)
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public string? AccentColor { get; set; }
        public string? DesignStyle { get; set; } // Modern, Classic, Minimalist, Bold
        public string? TargetAudience { get; set; } // Young, Professional, Family, Luxury
        public string? Tone { get; set; } // Formal, Friendly, Creative, Professional

        // Navigation preference
        public string PageController { get; set; } = "Home"; // Default controller to load

        // BİR sitenin, ÇOK sayfası olur (İlişki)
        public ICollection<Page> Pages { get; set; } = new List<Page>();
    }
}
