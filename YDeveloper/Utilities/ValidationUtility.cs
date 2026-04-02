namespace YDeveloper.Utilities
{
    public static class ValidationUtility
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                   && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public static bool IsValidPhoneNumber(string phone)
        {
            phone = phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
            return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^(\+90|0)?5\d{9}$");
        }
    }
}
