namespace YDeveloper.Constants
{
    public static class ApiRoutes
    {
        public const string Base = "/api";
        public const string Auth = Base + "/auth";
        public const string Sites = Base + "/sites";
        public const string Pages = Base + "/pages";
        public const string Templates = Base + "/templates";
        public const string Analytics = Base + "/analytics";
    }

    public static class HttpMethods
    {
        public const string Get = "GET";
        public const string Post = "POST";
        public const string Put = "PUT";
        public const string Delete = "DELETE";
        public const string Patch = "PATCH";
    }

    public static class ContentTypes
    {
        public const string Json = "application/json";
        public const string Html = "text/html";
        public const string Xml = "application/xml";
    }
}
