namespace YDeveloper.Extensions
{
    public static class StringExtensions
    {
        public static string ToSlug(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            value = value.ToLowerInvariant();
            value = System.Text.RegularExpressions.Regex.Replace(value, @"[çÇ]", "c");
            value = System.Text.RegularExpressions.Regex.Replace(value, @"[ğĞ]", "g");
            value = System.Text.RegularExpressions.Regex.Replace(value, @"[ıİ]", "i");
            value = System.Text.RegularExpressions.Regex.Replace(value, @"[öÖ]", "o");
            value = System.Text.RegularExpressions.Regex.Replace(value, @"[şŞ]", "s");
            value = System.Text.RegularExpressions.Regex.Replace(value, @"[üÜ]", "u");
            value = System.Text.RegularExpressions.Regex.Replace(value, @"[^a-z0-9\s-]", "");
            value = System.Text.RegularExpressions.Regex.Replace(value, @"\s+", " ").Trim();
            value = System.Text.RegularExpressions.Regex.Replace(value, @"\s", "-");
            
            return value;
        }

        public static string Truncate(this string value, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength) return value;
            return value.Substring(0, maxLength) + suffix;
        }
    }

    public static class DateTimeExtensions
    {
        public static string ToRelativeTime(this DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;

            if (timeSpan.TotalSeconds < 60) return "az önce";
            if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} dakika önce";
            if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours} saat önce";
            if (timeSpan.TotalDays < 30) return $"{(int)timeSpan.TotalDays} gün önce";
            if (timeSpan.TotalDays < 365) return $"{(int)(timeSpan.TotalDays / 30)} ay önce";
            return $"{(int)(timeSpan.TotalDays / 365)} yıl önce";
        }
    }
}
