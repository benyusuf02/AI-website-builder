using System.ComponentModel.DataAnnotations;

namespace YDeveloper.Models
{
    public class Page
    {
        [Key]
        public int Id { get; set; }

        // Bu sayfa hangi siteye ait? (Site.cs ile bağlantı)
        public int SiteId { get; set; }
        public Site? Site { get; set; }

        // Sayfayı oluşturan kullanıcı
        public string UserId { get; set; } = string.Empty;

        // URL uzantısı (Örn: "/hakkimizda" veya anasayfa için "/")
        public string Slug { get; set; } = "/";

        // YAPAY ZEKANIN ÜRETTİĞİ HTML KODU BURADA SAKLANACAK
        public string HtmlContent { get; set; } = string.Empty;

        // SEO Başlığı (Browser sekmesinde görünen yazı)
        public string MetaTitle { get; set; } = "Yeni Sayfa";

        // SEO Açıklaması (Google aramalarında çıkan alt yazı)
        public string MetaDescription { get; set; } = string.Empty;

        // Yayınlama durumu
        public bool IsPublished { get; set; } = false;
        public string? PublishedUrl { get; set; }
        public DateTime? LastPublishedAt { get; set; }

        // Zaman damgaları
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
