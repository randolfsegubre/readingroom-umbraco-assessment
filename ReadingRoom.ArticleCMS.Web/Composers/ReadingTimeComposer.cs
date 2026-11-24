using ReadingRoom.ArticleCMS.Core.ReadingTime;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using ReadingRoom.ArticleCMS.Core.Constants;

namespace ReadingRoom.ArticleCMS.Web.Composers;

/// <summary>
/// Registers the reading time service with the dependency injection container.
/// </summary>
public sealed class ReadingTimeComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        ReadingTimeOptions options = new ReadingTimeOptions();
        builder.Config.GetSection(ConfigurationSections.ReadingTime).Bind(options);

        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton<IReadingTimeService, ReadingTimeService>();
    }
}
