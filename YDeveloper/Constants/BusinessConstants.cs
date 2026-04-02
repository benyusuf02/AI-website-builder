namespace YDeveloper.Constants
{
    public static class FileConstants
    {
        public const long MaxImageSizeMb = 5;
        public const long MaxDocumentSizeMb = 10;
        
        public static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        public static readonly string[] AllowedDocumentExtensions = { ".pdf", ".doc", ".docx", ".txt" };
    }

    public static class ValidationConstants
    {
        public const int MinPasswordLength = 8;
        public const int MaxPasswordLength = 100;
        public const int MinUsernameLength = 3;
        public const int MaxUsernameLength = 50;
    }

    public static class BusinessConstants
    {
        public const int TrialDays = 14;
        public const int MaxSitesStarter = 1;
        public const int MaxSitesPro = 5;
        public const int MaxSitesEnterprise = -1; // Unlimited
    }
}
