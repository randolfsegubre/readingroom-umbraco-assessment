namespace ReadingRoom.ArticleCMS.Data.Models.ApiModels;

/// <summary>
/// The numbers the dashboard shows, one entry for each language of the site.
/// </summary>
public sealed class ArticleStatsResponse
{
    public List<LanguageStatsResponse> Languages { get; set; } = new List<LanguageStatsResponse>();
}
