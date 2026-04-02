using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;

namespace YDeveloper.Services
{
    public static class StringHelper
    {
        public static string Slugify(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            text = text.Trim().ToLower(new CultureInfo("en-US")); // Force lowercase

            // Remove diacritics
            text = RemoveDiacritics(text);

            // Invalid chars
            text = Regex.Replace(text, @"[^a-z0-9\s-]", "");

            // Convert multiple spaces into one space
            text = Regex.Replace(text, @"\s+", " ").Trim();

            // Replace space with hyphen
            text = Regex.Replace(text, @"\s", "-");

            return text;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
