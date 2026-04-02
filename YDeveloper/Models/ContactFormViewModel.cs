using System.ComponentModel.DataAnnotations;

namespace YDeveloper.Models
{
    public class ContactFormViewModel
    {
        [Required(ErrorMessage = "Ad Soyad gerekli")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta gerekli")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Konu gerekli")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mesaj gerekli")]
        [MinLength(10, ErrorMessage = "Mesaj en az 10 karakter olmalı")]
        public string Message { get; set; } = string.Empty;
    }

    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Şifre en az {2} karakter olmalı", MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class ContactMessage
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public string? AdminNotes { get; set; }
    }
}
