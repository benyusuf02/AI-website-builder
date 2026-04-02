namespace YDeveloper.Extensions
{
    public static class HttpContextExtensions
    {
        public static string? GetUserId(this HttpContext context)
        {
            return context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }

        public static string? GetUserEmail(this HttpContext context)
        {
            return context.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        }

        public static string GetClientIp(this HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        public static bool IsAjaxRequest(this HttpContext context)
        {
            return context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}
