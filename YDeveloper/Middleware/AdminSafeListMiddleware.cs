using System.Net;

namespace YDeveloper.Middleware
{
    public class AdminSafeListMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AdminSafeListMiddleware> _logger;
        private readonly string _adminSafeList;

        public AdminSafeListMiddleware(RequestDelegate next, ILogger<AdminSafeListMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _adminSafeList = configuration["AdminSafeList"]
                ?? throw new InvalidOperationException("AdminSafeList configuration is missing.");
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/Admin"))
            {
                var remoteIp = context.Connection.RemoteIpAddress;
                _logger.LogInformation($"Request to Admin area from IP: {remoteIp}");

                if (remoteIp != null)
                {
                    string[] safeList = _adminSafeList.Split(';');
                    var bytes = remoteIp.GetAddressBytes();
                    bool badIp = true;

                    foreach (var address in safeList)
                    {
                        if (IPAddress.TryParse(address, out var testIp))
                        {
                            if (testIp.GetAddressBytes().SequenceEqual(bytes))
                            {
                                badIp = false;
                                break;
                            }
                        }
                    }

                    if (badIp)
                    {
                        _logger.LogWarning($"Forbidden Request from Remote IP address: {remoteIp}");
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
