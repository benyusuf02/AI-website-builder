using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace YDeveloper.Filters
{
    /// <summary>
    /// Request logging filter - tracks all requests
    /// </summary>
    public class RequestLoggingAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<RequestLoggingAttribute>>();
            
            logger.LogInformation(
                "Request: {Method} {Path} | User: {User} | IP: {IP}",
                context.HttpContext.Request.Method,
                context.HttpContext.Request.Path,
                context.HttpContext.User?.Identity?.Name ?? "Anonymous",
                context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
        }
    }

    /// <summary>
    /// Model state logging for debugging
    /// </summary>
    public class ModelStateValidatorAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ModelStateValidatorAttribute>>();
                var errors = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                
                logger.LogWarning("Model validation failed: {Errors}", string.Join(", ", errors));
            }
        }
    }
}
