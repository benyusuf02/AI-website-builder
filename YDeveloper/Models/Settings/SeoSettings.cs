namespace YDeveloper.Models.Settings
{
    public class SeoSettings
    {
        public string DefaultTitle { get; set; } = "YDeveloper - AI Web Sitesi Oluşturucu";
        public string DefaultDescription { get; set; } = "Yapay zeka ile dakikalar içinde profesyonel web sitenizi oluşturun";
        public string DefaultKeywords { get; set; } = "web sitesi, yapay zeka, AI, site kurma";
        public string DefaultOgImage { get; set; } = "/images/og-default.jpg";
    }

    public class SocialMediaSettings
    {
        public string? FacebookUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? LinkedInUrl { get; set; }
    }
}
