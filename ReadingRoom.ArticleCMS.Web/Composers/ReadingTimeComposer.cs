using ReadingRoom.ArticleCMS.Web.Adapters;
using Umbraco.Cms.Core.Composing;

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
        builder.Services.AddScoped<IReadingSpeedSource, SiteSettingsReadingSpeedSource>();
        builder.Services.AddScoped<IReadingTimeService, ReadingTimeService>();
    }
}
