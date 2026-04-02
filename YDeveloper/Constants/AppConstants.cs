namespace YDeveloper.Constants
{
    /// <summary>
    /// Uygulama genelinde kullanılan sabit değerler
    /// </summary>
    public static class AppConstants
    {
        // Route Names
        public const string DefaultRoute = "default";
        public const string AdminRoute = "admin";
        
        // Session Keys
        public const string OnboardingDataKey = "OnboardingData";
        public const string UserPreferencesKey = "UserPreferences";
        
        // Cache Keys
        public const string PricingPackagesCacheKey = "pricing_packages";
        public const string SystemSettingsCacheKey = "system_settings";
        
        // Configuration Keys
        public const string GeminiApiKeyConfig = "AI:GeminiApiKey";
        public const string PaymentApiKeyConfig = "Payment:ApiKey";
        public const string PaymentSecretKeyConfig = "Payment:SecretKey";
        public const string AwsAccessKeyConfig = "AWS:AccessKey";
        public const string AwsSecretKeyConfig = "AWS:SecretKey";
        
        // Default Values
        public const int DefaultPageSize = 20;
        public const int DefaultCacheMinutes = 60;
        public const int DefaultSessionMinutes = 30;
        
        // Package Types
        public const string PackageStarter = "starter";
        public const string PackagePro = "pro";
        public const string PackageEnterprise = "enterprise";
        
        // User Roles
        public const string RoleAdmin = "Admin";
        public const string RoleModerator = "Moderator";
        public const string RoleUser = "User";
    }
    
    /// <summary>
    /// Hata mesajları - i18n'e hazır
    /// </summary>
    public static class ErrorMessages
    {
        public const string GenericError = "Bir hata oluştu. Lütfen tekrar deneyin.";
        public const string NotFound = "Aradığınız kayıt bulunamadı.";
        public const string Unauthorized = "Bu işlem için yetkiniz yok.";
        public const string ValidationFailed = "Girdiğiniz bilgileri kontrol edin.";
        public const string DatabaseError = "Veritabanı hatası oluştu.";
        public const string ExternalServiceError = "Dış servis şu an cevap vermiyor.";
        public const string PaymentFailed = "Ödeme işlemi başarısız oldu.";
        public const string DomainNotAvailable = "Bu domain kullanılamıyor.";
        public const string SessionExpired = "Oturumunuz sona erdi. Lütfen tekrar giriş yapın.";
    }
    
    /// <summary>
    /// Başarı mesajları
    /// </summary>
    public static class SuccessMessages
    {
        public const string SaveSuccess = "Değişiklikler kaydedildi.";
        public const string CreateSuccess = "Başarıyla oluşturuldu.";
        public const string UpdateSuccess = "Başarıyla güncellendi.";
        public const string DeleteSuccess = "Başarıyla silindi.";
        public const string PaymentSuccess = "Ödeme işlemi tamamlandı.";
        public const string EmailSent = "E-posta başarıyla gönderildi.";
    }
}
