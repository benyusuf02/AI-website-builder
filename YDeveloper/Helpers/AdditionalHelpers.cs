namespace YDeveloper.Helpers
{
    public static class ImageHelper
    {
        public static bool IsValidImageFormat(string extension)
        {
            var validFormats = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
            return validFormats.Contains(extension.ToLowerInvariant());
        }

        public static string GetContentType(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".svg" => "image/svg+xml",
                _ => "application/octet-stream"
            };
        }
    }

    public static class MoneyHelper
    {
        public static string FormatCurrency(decimal amount, string currency = "TRY")
        {
            return currency switch
            {
                "TRY" => $"₺{amount:N2}",
                "USD" => $"${amount:N2}",
                "EUR" => $"€{amount:N2}",
                _ => $"{amount:N2} {currency}"
            };
        }
    }
}
