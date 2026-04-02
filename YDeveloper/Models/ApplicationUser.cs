using Microsoft.AspNetCore.Identity;
using System;

namespace YDeveloper.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // SaaS Profile Fields
        [PersonalData]
        public string? BusinessName { get; set; }
        [PersonalData]
        public string? Industry { get; set; }
        public int OnboardingStep { get; set; } = 0; // 0: New, 1: Domain, 2: Template, 3: Completed

        // Legal Consent
        public bool HasAgreedToTerms { get; set; }
        public DateTime? TermsAgreementDate { get; set; }

        // Billing Info
        [PersonalData]
        public string? Address { get; set; }
        [PersonalData]
        public string? City { get; set; }
        [PersonalData]
        public string? Country { get; set; }
        [PersonalData]
        public string? ZipCode { get; set; }
        [PersonalData]
        public string? IdentityNumber { get; set; }

        // Membership / Quota
        public int? CurrentPackageId { get; set; }
        public DateTime? PackageExpiryDate { get; set; }
        public int WebsiteLimit { get; set; } = 1;

        public string? CardUserKey { get; set; } // Iyzico Card Storage Token
        public string? ProfilePictureUrl { get; set; }

        // Internal Admin Usage
        public string? AdminNote { get; set; }

        public DateTime? LastLoginDate { get; set; }
    }
}
