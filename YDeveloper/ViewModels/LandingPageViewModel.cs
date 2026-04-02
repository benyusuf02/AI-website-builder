using YDeveloper.Models;

namespace YDeveloper.ViewModels
{
    public class LandingPageViewModel
    {
        public ContentItem? HeroSection { get; set; }
        public List<ContentItem> Features { get; set; } = new List<ContentItem>();
        public List<PricingPackage> PricingPackages { get; set; } = new List<PricingPackage>();
    }
}
