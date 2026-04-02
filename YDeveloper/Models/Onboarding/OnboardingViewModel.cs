using System.ComponentModel.DataAnnotations;

namespace YDeveloper.Models.Onboarding
{
    public class OnboardingViewModel
    {
        // Step 1: Site Bilgileri
        [Required(ErrorMessage = "Site adı zorunludur")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Site adı 3-100 karakter olmalıdır")]
        [RegularExpression(@"^[a-zA-Z0-9çÇğĞıİöÖşŞüÜ\s-]+$", ErrorMessage = "Geçersiz karakterler içeriyor")]
        public string SiteName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Site açıklaması zorunludur")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Açıklama 10-500 karakter olmalıdır")]
        public string SiteDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sektör seçimi zorunludur")]
        public string Industry { get; set; } = string.Empty;

        // Step 2: Tasarım Tercihi
        [Required]
        public string ColorPalette { get; set; } = "blue";

        [Required]
        public string DesignStyle { get; set; } = "modern";

        // Step 3: AI İçerik
        [StringLength(1000, ErrorMessage = "Prompt çok uzun")]
        public string ContentPrompt { get; set; } = string.Empty;

        // Step 4: Domain & Paket
        [Required(ErrorMessage = "Subdomain zorunludur")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Subdomain 3-50 karakter olmalıdır")]
        [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Sadece küçük harf, rakam ve tire kullanılabilir")]
        public string Subdomain { get; set; } = string.Empty;

        public bool IsCustomDomain { get; set; } = false;

        [Url(ErrorMessage = "Geçerli bir domain girin")]
        [StringLength(100)]
        public string CustomDomain { get; set; } = string.Empty;

        [Required(ErrorMessage = "Paket seçimi zorunludur")]
        [RegularExpression("starter|pro|enterprise", ErrorMessage = "Geçersiz paket")]
        public string PackageType { get; set; } = "starter";

        // Progress Tracking
        [Range(1, 5)]
        public int CurrentStep { get; set; } = 1;

        public int TotalSteps { get; set; } = 5;
    }

    public class Step1ViewModel
    {
        [Required(ErrorMessage = "Site adı zorunludur")]
        [StringLength(100, MinimumLength = 3)]
        public string SiteName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Site açıklaması zorunludur")]
        [StringLength(500, MinimumLength = 10)]
        public string SiteDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sektör seçimi zorunludur")]
        public string Industry { get; set; } = string.Empty;
    }

    public class Step2ViewModel
    {
        [Required]
        public string ColorPalette { get; set; } = "blue";

        [Required]
        public string DesignStyle { get; set; } = "modern";
    }

    public class Step3ViewModel
    {
        [StringLength(1000)]
        public string ContentPrompt { get; set; } = string.Empty;
    }

    public class Step4ViewModel
    {
        [Required(ErrorMessage = "Subdomain zorunludur")]
        [StringLength(50, MinimumLength = 3)]
        [RegularExpression(@"^[a-z0-9-]+$")]
        public string Subdomain { get; set; } = string.Empty;

        public bool IsCustomDomain { get; set; } = false;

        [Url]
        [StringLength(100)]
        public string CustomDomain { get; set; } = string.Empty;

        [Required(ErrorMessage = "Paket seçimi zorunludur")]
        public string PackageType { get; set; } = "starter";
    }
}
