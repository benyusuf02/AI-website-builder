using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using YDeveloper.Constants;

namespace YDeveloper.Filters
{
    /// <summary>
    /// Model validation filter - otomatik validation
    /// </summary>
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .Select(x => new
                    {
                        Field = x.Key,
                        Messages = x.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                    })
                    .ToList();

                context.Result = new BadRequestObjectResult(new
                {
                    Success = false,
                    Message = ErrorMessages.ValidationFailed,
                    Errors = errors
                });
            }
        }
    }

    /// <summary>
    /// Anti-XSS input sanitization attribute
    /// </summary>
    public class SanitizeInputAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var argument in context.ActionArguments)
            {
                if (argument.Value is string stringValue)
                {
                    // Basic XSS protection - HTML encode
                    context.ActionArguments[argument.Key] = System.Net.WebUtility.HtmlEncode(stringValue);
                }
            }
        }
    }

    /// <summary>
    /// API Key authentication filter
    /// </summary>
    public class ApiKeyAuthAttribute : ActionFilterAttribute
    {
        private const string ApiKeyHeaderName = "X-API-Key";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult(new { Message = "API Key missing" });
                return;
            }

            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var validApiKey = configuration["ApiSettings:ValidApiKey"];

            if (string.IsNullOrEmpty(validApiKey) || !validApiKey.Equals(extractedApiKey.ToString()))
            {
                context.Result = new UnauthorizedObjectResult(new { Message = "Invalid API Key" });
            }
        }
    }
}
