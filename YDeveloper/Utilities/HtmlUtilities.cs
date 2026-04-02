using System.Text.RegularExpressions;

namespace YDeveloper.Utilities
{
    public static class HtmlSanitizer
    {
        private static readonly string[] AllowedTags = { "p", "br", "strong", "em", "u", "a", "img", "h1", "h2", "h3", "ul", "ol", "li" };

        public static string Sanitize(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;

            // Remove script tags
            html = Regex.Replace(html, @"<script[^>]*>[\s\S]*?</script>", "", RegexOptions.IgnoreCase);
            
            // Remove on* event handlers
            html = Regex.Replace(html, @"\s*on\w+\s*=\s*[""'][^""']*[""']", "", RegexOptions.IgnoreCase);
            
            // Remove javascript: protocol
            html = Regex.Replace(html, @"javascript:", "", RegexOptions.IgnoreCase);
            
            return html;
        }

        public static string StripHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;
            return Regex.Replace(html, @"<[^>]+>", "");
        }
    }

    public static class SlugGenerator
    {
        public static string Generate(string text, int maxLength = 50)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            text = text.ToLowerInvariant();
            text = text.Replace("ç", "c").Replace("ğ", "g").Replace("ı", "i")
                       .Replace("ö", "o").Replace("ş", "s").Replace("ü", "u");
            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
            text = Regex.Replace(text, @"\s+", " ").Trim();
            text = text.Substring(0, Math.Min(text.Length, maxLength)).Trim();
            text = Regex.Replace(text, @"\s", "-");

            return text;
        }
    }
}
