using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace YDeveloper.Middleware
{
    /// <summary>
    /// Global exception handler middleware - tüm yakalanmamış hataları loglar ve kullanıcıya dostane mesaj gösterir
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "İşlenmeyen Hata: {Message} | Kullanıcı: {User} | URL: {Url}",
                    ex.Message,
                    context.User?.Identity?.Name ?? "Anonim",
                    context.Request.Path);

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "text/html";
            var (statusCode, userMessage) = GetUserFriendlyError(exception);
            context.Response.StatusCode = statusCode;

            if (_env.IsDevelopment())
            {
                var detailedError = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <title>🔥 Geliştirme Ortamı Hatası</title>
                        <style>
                            body {{ font-family: 'Segoe UI', sans-serif; padding: 40px; background: #1a1a1a; color: #fff; }}
                            .error-box {{ background: #2d2d2d; padding: 30px; border-radius: 8px; border-left: 4px solid #ef4444; }}
                            h1 {{ color: #ef4444; margin-top: 0; }}
                            pre {{ background: #1a1a1a; padding: 20px; overflow-x: auto; border-radius: 4px; }}
                        </style>
                    </head>
                    <body>
                        <div class='error-box'>
                            <h1>🔥 Geliştirme Ortamı Hatası</h1>
                            <h3>{exception.GetType().Name}</h3>
                            <p><strong>Kullanıcı Mesajı:</strong> {userMessage}</p>
                            <p><strong>Teknik Mesaj:</strong> {exception.Message}</p>
                            <pre>{exception.StackTrace}</pre>
                        </div>
                    </body>
                    </html>";
                await context.Response.WriteAsync(detailedError);
            }
            else
            {
                context.Response.Redirect($"/Home/Error?message={Uri.EscapeDataString(userMessage)}");
            }
        }

        private (int statusCode, string message) GetUserFriendlyError(Exception exception)
        {
            return exception switch
            {
                UnauthorizedAccessException => (401, YDeveloper.Constants.ErrorMessages.Unauthorized),
                KeyNotFoundException or FileNotFoundException => (404, YDeveloper.Constants.ErrorMessages.NotFound),
                InvalidOperationException => (400, YDeveloper.Constants.ErrorMessages.ValidationFailed),
                TimeoutException => (408, YDeveloper.Constants.ErrorMessages.ExternalServiceError),
                _ => (500, YDeveloper.Constants.ErrorMessages.GenericError)
            };
        }
    }
}
