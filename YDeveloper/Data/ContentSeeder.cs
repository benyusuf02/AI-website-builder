using Microsoft.EntityFrameworkCore;
using YDeveloper.Models;

namespace YDeveloper.Data
{
    public static class ContentSeeder
    {
        public static async Task SeedAsync(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<YDeveloperContext>();

                // Seed ContentItems for dynamic homepage/about/contact
                if (!await context.ContentItems.AnyAsync())
                {
                    var contentItems = new List<ContentItem>
                    {
                        new ContentItem { Section = "homepage", Key = "hero_badge", Title = "Yapay Zeka Destekli Gelecek", Description = "Google Gemini AI ile web sitesi inşasında yeni bir dönem.", Icon = "stars" },
                        new ContentItem { Section = "homepage", Key = "hero_title_1", Title = "Yapay Zeka ile", Description = "Ana Başlık Satır 1" },
                        new ContentItem { Section = "homepage", Key = "hero_title_2", Title = "Geleceği İnşa Et", Description = "Ana Başlık Satır 2" },
                        new ContentItem { Section = "homepage", Key = "hero_subtitle", Title = "Kod yazmadan, saniyeler içinde dünya standartlarında bir web sitesine sahip olun.", Description = "Alt Başlık" },
                        new ContentItem { Section = "homepage", Key = "hero_cta_primary", Title = "Hemen Başla", Description = "Buton Metni" },

                        new ContentItem { Section = "homepage", Key = "stat_users_label", Title = "Vizyoner Kullanıcı", Description = "İstatistik 1" },
                        new ContentItem { Section = "homepage", Key = "stat_sites_label", Title = "Dijital Varlık", Description = "İstatistik 2" },
                        new ContentItem { Section = "homepage", Key = "stat_uptime_label", Title = "Kesintisiz Güç", Description = "İstatistik 3" },

                        // Features
                        new ContentItem { Section = "features", Key = "feature_1", Title = "Nebula UI/UX", Description = "Göz yormayan, sürükleyici ve kullanıcı dostu arayüzler.", Icon = "palette" },
                        new ContentItem { Section = "features", Key = "feature_2", Title = "Gemini IQ+", Description = "İçeriklerinizi ve tasarımınızı optimize eden yapay zeka çekirdeği.", Icon = "cpu" },
                        new ContentItem { Section = "features", Key = "feature_3", Title = "Kuantum Hız", Description = "Saniyeler içinde yayınlanan, her cihazda ışık hızında açılan sayfalar.", Icon = "lightning-charge" },
                        new ContentItem { Section = "features", Key = "feature_4", Title = "Global Scaling", Description = "Dünya genelinde 100+ veri merkezinde otomatik dağıtım.", Icon = "globe2" },
                        new ContentItem { Section = "features", Key = "feature_5", Title = "Siber Kalkan", Description = "Kurumsal düzeyde DDoS koruması ve SSL şifreleme.", Icon = "shield-lock" },
                        new ContentItem { Section = "features", Key = "feature_6", Title = "Canlı Analitik", Description = "Anlık ziyaretçi veri akışı ve yapay zeka tabanlı içgörüler.", Icon = "activity" },
                        new ContentItem { Section = "features", Key = "feature_7", Title = "Oto-Pilot SEO", Description = "Arama motorlarında zirveye oynamanız için otomatik optimizasyon.", Icon = "search" },
                        new ContentItem { Section = "features", Key = "feature_8", Title = "Nöral Ağ", Description = "Siteniz öğrendikçe gelişir, kullanıcı davranışına göre evrimleşir.", Icon = "share" },
                        new ContentItem { Section = "features", Key = "feature_9", Title = "Kuantum Güvenlik", Description = "Askeri düzeyde şifreleme ile verileriniz kırılamaz.", Icon = "shield-fill" },
                        new ContentItem { Section = "features", Key = "feature_10", Title = "Bulut Senkron", Description = "Tüm cihazlarda anlık veri eşitleme ve yedekleme.", Icon = "cloud-check" },
                        new ContentItem { Section = "features", Key = "feature_11", Title = "AI Analitik", Description = "Ziyaretçi davranışlarını analiz edip satışları artıran öneriler.", Icon = "bar-chart-line" },
                        new ContentItem { Section = "features", Key = "feature_12", Title = "Modüler Yapı", Description = "Lego gibi birleşen parçalarla sınırsız tasarım özgürlüğü.", Icon = "grid-3x3-gap" },
                        new ContentItem { Section = "features", Key = "feature_13", Title = "API First", Description = "Herhangi bir servise saniyeler içinde entegre olun.", Icon = "code-slash" },
                        new ContentItem { Section = "features", Key = "feature_14", Title = "Sıfır Gecikme", Description = "Kenar sunucularla (Edge) ışık hızında içerik dağıtımı.", Icon = "speedometer" },
                        new ContentItem { Section = "features", Key = "feature_15", Title = "Adaptif Arayüz", Description = "Ekran boyutuna göre kendini yeniden çizen akıllı tasarım.", Icon = "phone-landscape" },
                        new ContentItem { Section = "features", Key = "feature_16", Title = "Sesli Kontrol", Description = "Sitenizi ses komutlarıyla yönetin ve düzenleyin.", Icon = "mic" },

                        // CTA
                        new ContentItem { Section = "homepage", Key = "cta_title", Title = "Kendi Geleceğini Tasarla", Description = "Alt CTA Başlık" },
                        new ContentItem { Section = "homepage", Key = "cta_subtitle", Title = "Gelecek bekleyenler için değil, inşa edenler içindir.", Description = "Alt CTA Alt Başlık" },
                        
                        // About Page
                        new ContentItem { Section = "about", Key = "about_hero_tag", Title = "Vizyonumuz", Description = "Hero Etiket" },
                        new ContentItem { Section = "about", Key = "about_hero_title", Title = "Geleceği Birlikte Şekillendiriyoruz", Description = "Hero Başlık" },
                        new ContentItem { Section = "about", Key = "about_hero_desc", Title = "YDeveloper, web tasarımının sınırlarını yapay zeka ile zorlayan, teknoloji ve estetiği buluşturan bir inovasyon merkezidir.", Description = "Hero Açıklama" },
                        new ContentItem { Section = "about", Key = "about_mission_title", Title = "Misyonumuz", Description = "Misyon Başlık" },
                        new ContentItem { Section = "about", Key = "about_mission_desc", Title = "Web sitesi oluşturmayı teknik bir engel olmaktan çıkarıp, herkes için yaratıcı bir sürece dönüştürmek.", Description = "Misyon Açıklama" },
                        new ContentItem { Section = "about", Key = "about_vision_title", Title = "Vizyonumuz", Description = "Vizyon Başlık" },
                        new ContentItem { Section = "about", Key = "about_vision_desc", Title = "Dünyanın en akıllı ve en estetik AI tasarım platformu olmak.", Description = "Vizyon Açıklama" },

                        // Contact Page
                        new ContentItem { Section = "contact", Key = "contact_hero_tag", Title = "İletişim", Description = "Hero Etiket" },
                        new ContentItem { Section = "contact", Key = "contact_hero_title", Title = "Bizimle Bağlantıda Kalın", Description = "Hero Başlık" },
                        new ContentItem { Section = "contact", Key = "contact_address", Title = "İstanbul, Türkiye", Description = "Adres" },
                        new ContentItem { Section = "contact", Key = "contact_email", Title = "info@ydeveloper.com", Description = "E-posta" },
                        new ContentItem { Section = "contact", Key = "contact_phone", Title = "+90 (555) 123 45 67", Description = "Telefon" },

                        // Legal Pages
                        new ContentItem {
                            Section = "legal",
                            Key = "terms_of_use",
                            Title = "Kullanım Koşulları",
                            Description = "Kullanım Şartları Metni",
                            HtmlContent = "<h1>Kullanım Koşulları</h1><p>Bu siteyi kullanarak aşağıdaki koşulları kabul etmiş sayılırsınız...</p>"
                        },
                        new ContentItem {
                            Section = "legal",
                            Key = "privacy_policy",
                            Title = "Gizlilik Politikası ve KVKK",
                            Description = "Gizlilik Politikası Metni",
                            HtmlContent = "<h1>Gizlilik Politikası</h1><p>Verileriniz bizimle güvende...</p>"
                        },
                        new ContentItem {
                            Section = "legal",
                            Key = "distance_sales",
                            Title = "Mesafeli Satış Sözleşmesi",
                            Description = "Mesafeli Satış Sözleşmesi Metni",
                            HtmlContent = "<h1>Mesafeli Satış Sözleşmesi</h1><p>Satıcı ve alıcı arasındaki yükümlülükler...</p>"
                        }
                    };

                    await context.ContentItems.AddRangeAsync(contentItems);
                    await context.SaveChangesAsync();
                }

                // Seed BlogPosts
                if (!await context.BlogPosts.AnyAsync())
                {
                    var posts = new List<BlogPost>
                    {
                        new BlogPost
                        {
                            Title = "YDeveloper'a Hoş Geldiniz",
                            Slug = "ydeveloper-hos-geldiniz",
                            Summary = "Yapay zeka ile profesyonel web siteleri oluşturun.",
                            Content = "<p>YDeveloper ile tanışın.</p>",
                            MetaTitle = "YDeveloper: AI Site Oluşturucu",
                            MetaDescription = "AI destekli platform",
                            Tags = "YDeveloper, AI",
                            IsPublished = true,
                            PublishedAt = DateTime.UtcNow,
                            CoverImageUrl = "https://images.unsplash.com/photo-1519389950473-47ba0277781c?w=1000"
                        }
                    };

                    await context.BlogPosts.AddRangeAsync(posts);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
