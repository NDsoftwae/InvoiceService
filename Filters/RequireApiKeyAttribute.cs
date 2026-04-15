using InvoiceService.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace InvoiceService.Filters;

public sealed class RequireApiKeyAttribute : Attribute, IAsyncActionFilter
{
    private readonly ApiKeySettings _settings;

    public RequireApiKeyAttribute(IOptions<ApiKeySettings> settings)
    {
        _settings = settings.Value;
    }

    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(_settings.HeaderName, out var suppliedKey))
        {
            context.Result = new UnauthorizedObjectResult(new { error = $"Missing required header '{_settings.HeaderName}'." });
            return Task.CompletedTask;
        }

        if (!string.Equals(suppliedKey.ToString(), _settings.Value, StringComparison.Ordinal))
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Invalid API key." });
            return Task.CompletedTask;
        }

        return next();
    }
}
