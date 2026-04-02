namespace YDeveloper.Utilities
{
    public static class CryptoUtility
    {
        public static string GenerateRandomToken(int length = 32)
        {
            var bytes = new byte[length];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }

        public static string HashSHA256(string input)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }

    public static class StringUtility
    {
        public static string TruncateWithEllipsis(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength) return value;
            return value.Substring(0, maxLength - 3) + "...";
        }

        public static string ToTitleCase(string value)
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
        }
    }
}
