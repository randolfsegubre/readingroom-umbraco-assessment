using ReadingRoom.ArticleCMS.Web.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using Umbraco.Cms.Core.Composing;

namespace ReadingRoom.ArticleCMS.Web.Composers;

/// <summary>
/// Configures the Swagger documents at /umbraco/swagger. Umbraco serves that page only
/// outside the Production environment.
/// </summary>
public sealed class SwaggerComposer : IComposer
{
    private const string DeliveryApiDocumentName = "delivery";

    public void Compose(IUmbracoBuilder builder)
    {
        string[] documentsThatNeedTheApiKey = new[] { DeliveryApiDocumentName };

        builder.Services.Configure<SwaggerGenOptions>(options =>
        {
            options.DocumentFilter<ApiKeySecurityDocumentFilter>(new object[] { documentsThatNeedTheApiKey });
        });
    }
}
