using ReadingRoom.ArticleCMS.Web.Adapters;
using ReadingRoom.ArticleCMS.Web.Authentication;
using Umbraco.Cms.Core.Composing;

namespace ReadingRoom.ArticleCMS.Web.Composers;

/// <summary>
/// Registers what the articles API and the stats endpoint need.
/// </summary>
public sealed class ArticlesApiComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<ApiKeyAuthorizationFilter>();
        builder.Services.AddScoped<IPublishedArticleReader, PublishedArticleReader>();
    }
}
