using System.ComponentModel.DataAnnotations;

namespace YDeveloper.Models
{
    public class PricingPackage
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; } // Monthly Price
        public decimal YearlyPrice { get; set; } // Yearly Price

        public string Description { get; set; } = string.Empty;
        public string Features { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public List<string> FeaturesList { get; set; } = new List<string>();

        public bool IsActive { get; set; } = true;

        // Pricing Model: Setup (1 kez) + Monthly (her ay)
        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "decimal(18,2)")]
        public decimal SetupFee { get; set; } // Kurulum ücreti (1 kez)
        
        [System.ComponentModel.DataAnnotations.Schema.Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyFee { get; set; } // Aylık kiralama ücreti

        // Dynamic Display Fields
        public bool IsPopular { get; set; } = false;
        public string ButtonText { get; set; } = "Hemen Başla";
        public string BillingPeriod { get; set; } = "Aylık";

        // Quota
        public int WebsiteLimit { get; set; } = 1;
        public int MaxSites { get; set; } = 1; // Alias for WebsiteLimit
        public int MaxPagesPerSite { get; set; } = 10;
        public int DisplayOrder { get; set; } = 0;
    }
}
