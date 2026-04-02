namespace YDeveloper.Constants
{
    public static class CacheKeys
    {
        public const string PricingPackages = "cache:pricing:packages";
        public const string SystemSettings = "cache:system:settings";
        public const string UserSites = "cache:user:{0}:sites";
        public const string SitePages = "cache:site:{0}:pages";
        public const string SiteAnalytics = "cache:site:{0}:analytics";

        public static string GetUserSitesKey(string userId) => string.Format(UserSites, userId);
        public static string GetSitePagesKey(int siteId) => string.Format(SitePages, siteId);
        public static string GetSiteAnalyticsKey(int siteId) => string.Format(SiteAnalytics, siteId);
    }

    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Moderator = "Moderator";
        public const string User = "User";
    }

    public static class Policies
    {
        public const string AdminOnly = "AdminOnly";
        public const string ModeratorOrAdmin = "ModeratorOrAdmin";
    }
}
