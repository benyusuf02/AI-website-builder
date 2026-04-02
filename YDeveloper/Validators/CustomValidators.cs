using System.ComponentModel.DataAnnotations;

namespace YDeveloper.Validators
{
    public class SubdomainValidator : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string subdomain) return ValidationResult.Success;

            if (subdomain.Length < 3 || subdomain.Length > 50)
                return new ValidationResult("Subdomain 3-50 karakter olmalıdır.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(subdomain, @"^[a-z0-9-]+$"))
                return new ValidationResult("Subdomain sadece küçük harf, rakam ve tire içerebilir.");

            var reservedWords = new[] { "www", "api", "admin", "app", "mail", "ftp", "blog" };
            if (reservedWords.Contains(subdomain.ToLower()))
                return new ValidationResult("Bu subdomain rezerve edilmiştir.");

            return ValidationResult.Success;
        }
    }

    public class TurkishPhoneValidator : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string phone) return ValidationResult.Success;

            phone = phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
            
            if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^(\+90|0)?5\d{9}$"))
                return new ValidationResult("Geçerli bir Türkiye telefon numarası giriniz.");

            return ValidationResult.Success;
        }
    }
}
