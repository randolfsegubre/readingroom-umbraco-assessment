namespace ReadingRoom.ArticleCMS.Web.Adapters;

/// <summary>
/// Reads the reading speed from the Site Settings document that editors manage in the backoffice.
/// </summary>
public sealed class SiteSettingsReadingSpeedSource : IReadingSpeedSource
{
    private readonly IPublishedContentQuery _contentQuery;

    public SiteSettingsReadingSpeedSource(IPublishedContentQuery contentQuery)
    {
        _contentQuery = contentQuery;
    }

    public int? GetWordsPerMinute()
    {
        foreach (IPublishedContent root in _contentQuery.ContentAtRoot())
        {
            if (root is SiteSettings settings)
            {
                return settings.WordsPerMinute;
            }
        }

        return null;
    }
}
