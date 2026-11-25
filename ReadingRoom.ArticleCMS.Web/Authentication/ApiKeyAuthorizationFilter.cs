using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using ReadingRoom.ArticleCMS.Core.Security;
using Umbraco.Cms.Core.Configuration.Models;

namespace ReadingRoom.ArticleCMS.Web.Authentication;

/// <summary>
/// Refuses a request with the status 401 unless it carries the configured API key in the Api-Key header.
/// The key is the same one that protects the Delivery API (Umbraco:CMS:DeliveryApi:ApiKey).
/// </summary>
public sealed class ApiKeyAuthorizationFilter : IAuthorizationFilter
{
    private readonly IOptionsMonitor<DeliveryApiSettings> _deliveryApiSettings;

    public ApiKeyAuthorizationFilter(IOptionsMonitor<DeliveryApiSettings> deliveryApiSettings)
    {
        _deliveryApiSettings = deliveryApiSettings;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        string? providedKey = context.HttpContext.Request.Headers[ApiKeyAuthentication.HeaderName];
        string? configuredKey = _deliveryApiSettings.CurrentValue.ApiKey;

        if (!ApiKeyValidator.IsValid(configuredKey, providedKey))
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
