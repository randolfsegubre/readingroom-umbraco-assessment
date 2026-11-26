using Microsoft.OpenApi;
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
        string[] documentsThatNeedTheApiKey = new[] { DeliveryApiDocumentName, ReadingRoomApi.ArticlesDocumentName };
        string commentsFile = Path.Combine(AppContext.BaseDirectory, $"{typeof(SwaggerComposer).Assembly.GetName().Name}.xml");

        builder.Services.Configure<SwaggerGenOptions>(options =>
        {
            options.SwaggerDoc(ReadingRoomApi.ArticlesDocumentName, new OpenApiInfo
            {
                Title = "Reading Room Articles API",
                Version = "v1",
                Description = "A read-only API for the published articles."
            });
            options.DocumentFilter<ApiKeySecurityDocumentFilter>(new object[] { documentsThatNeedTheApiKey });
            options.IncludeXmlComments(commentsFile);
        });
    }
}
