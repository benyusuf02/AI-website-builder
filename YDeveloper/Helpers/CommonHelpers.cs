namespace YDeveloper.Helpers
{
    public static class UrlHelper
    {
        public static string GenerateSubdomainUrl(string subdomain)
        {
            return $"{subdomain}.ydeveloper.com";
        }

        public static bool IsValidSubdomain(string subdomain)
        {
            if (string.IsNullOrWhiteSpace(subdomain)) return false;
            if (subdomain.Length < 3 || subdomain.Length > 50) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(subdomain, @"^[a-z0-9-]+$");
        }
    }

    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            return new Microsoft.AspNetCore.Identity.PasswordHasher<object>().HashPassword(null!, password);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var result = new Microsoft.AspNetCore.Identity.PasswordHasher<object>()
                .VerifyHashedPassword(null!, hashedPassword, password);
            return result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success;
        }

        public static string GenerateRandomPassword(int length = 12)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    public static class PricingHelper
    {
        public static decimal CalculateDiscount(decimal price, decimal discountPercent)
        {
            return price * (discountPercent / 100);
        }

        public static decimal ApplyDiscount(decimal price, decimal discountPercent)
        {
            return price - CalculateDiscount(price, discountPercent);
        }

        public static decimal CalculateTax(decimal price, decimal taxRate = 18)
        {
            return price * (taxRate / 100);
        }
    }
}
