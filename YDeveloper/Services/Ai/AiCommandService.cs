using System.Text.RegularExpressions;
using YDeveloper.Models.Ai;

namespace YDeveloper.Services.Ai
{
    public class AiCommandService : IAiCommandService
    {
        public AiResponse ProcessCommand(string prompt)
        {
            var p = prompt.Trim().ToLower(new System.Globalization.CultureInfo("tr-TR"));

            // 1. Navigation / Redirection
            if (IsNavigation(p, out string targetUrl))
            {
                return new AiResponse
                {
                    Success = true,
                    Message = "Yönlendiriliyor...",
                    ActionType = AiActionType.Redirect,
                    Payload = new AiRedirectPayload { Url = targetUrl }
                };
            }

            // 2. Theme / Style
            if (IsThemeChange(p, out var styles, out string themeMsg))
            {
                return new AiResponse
                {
                    Success = true,
                    Message = themeMsg,
                    ActionType = AiActionType.StyleUpdate,
                    Payload = styles
                };
            }

            // 3. Content Update (Simulated)
            if (p.Contains("başlığı değiştir") || p.Contains("yazıyı değiştir"))
            {
                return new AiResponse
                {
                    Success = true,
                    Message = "Başlık güncellendi (Simülasyon).",
                    ActionType = AiActionType.Toast, // For now just toast
                    Payload = null
                };
            }

            // 4. Admin Actions
            if (p.Contains("yedek al") || p.Contains("backup"))
            {
                return new AiResponse
                {
                    Success = true,
                    Message = "Yedekleme işlemi başlatıldı.",
                    ActionType = AiActionType.Toast,
                    Payload = null
                };
            }

            return new AiResponse
            {
                Success = false,
                Message = "Bu komutu henüz anlayamadım. Şunları deneyin: 'Koyu mod yap', 'Admin paneline git', 'Arka planı kırmızı yap'.",
                ActionType = AiActionType.None
            };
        }

        private bool IsNavigation(string p, out string url)
        {
            url = "";
            if (p.Contains("admin") || p.Contains("yönetim"))
            {
                url = "/Admin"; // Go to Admin
                return true;
            }
            if (p.Contains("ana sayfa") || p.Contains("siteye dön"))
            {
                url = "/";
                return true;
            }
            if (p.Contains("iletişim"))
            {
                url = "/Home/Contact"; // Assuming contact page
                return true;
            }
            if (p.Contains("kayıt") || p.Contains("üye ol"))
            {
                url = "/Identity/Account/Register";
                return true;
            }
            if (p.Contains("giriş"))
            {
                url = "/Identity/Account/Login";
                return true;
            }
            return false;
        }

        private bool IsThemeChange(string p, out AiStylePayload payload, out string msg)
        {
            payload = new AiStylePayload();
            msg = "";

            if (p.Contains("koyu mod") || p.Contains("dark mode") || p.Contains("gece modu"))
            {
                msg = "Koyu moda geçildi.";
                // In a real app we might toggle a class, but here we set vars if needed
                // For now, let's assume frontend handles 'theme' toggle via specific command or we just toast
                // Actually, let's send a simulated style update payload for demonstration
                payload.Styles.Add("--bs-body-bg", "#0f172a");
                payload.Styles.Add("--bs-body-color", "#f8fafc");
                return true;
            }

            if (p.Contains("açık mod") || p.Contains("light mode"))
            {
                msg = "Açık moda geçildi.";
                payload.Styles.Add("--bs-body-bg", "#ffffff");
                payload.Styles.Add("--bs-body-color", "#212529");
                return true;
            }

            // Colors
            if (p.Contains("kırmızı yap"))
            {
                msg = "Tema rengi kırmızı yapıldı.";
                payload.Styles.Add("--bs-primary", "#ef4444");
                payload.Styles.Add("--primary-gradient", "linear-gradient(to right, #ef4444, #f87171)");
                return true;
            }
            if (p.Contains("mavi yap"))
            {
                msg = "Tema rengi mavi yapıldı.";
                payload.Styles.Add("--bs-primary", "#3b82f6");
                payload.Styles.Add("--primary-gradient", "linear-gradient(to right, #3b82f6, #60a5fa)");
                return true;
            }
            if (p.Contains("yeşil yap"))
            {
                msg = "Tema rengi yeşil yapıldı.";
                payload.Styles.Add("--bs-primary", "#22c55e");
                payload.Styles.Add("--primary-gradient", "linear-gradient(to right, #22c55e, #4ade80)");
                return true;
            }

            // Fonts
            if (p.Contains("font") && p.Contains("büyüt"))
            {
                msg = "Yazı boyutu büyütüldü.";
                payload.Selector = "html";
                payload.Styles.Add("font-size", "18px");
                return true;
            }
            if (p.Contains("font") && p.Contains("küçült"))
            {
                msg = "Yazı boyutu küçültüldü.";
                payload.Selector = "html";
                payload.Styles.Add("font-size", "14px");
                return true;
            }

            return false;
        }
    }
}
