using ReadingRoom.ArticleCMS.Core.Newsletter;
using ReadingRoom.ArticleCMS.Web.Adapters;
using Umbraco.Cms.Core.Composing;

namespace ReadingRoom.ArticleCMS.Web.Composers;

/// <summary>
/// Registers where newsletter sign-ups are saved.
/// </summary>
public sealed class NewsletterComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddScoped<INewsletterSignUpStore, DatabaseNewsletterSignUpStore>();
    }
}
