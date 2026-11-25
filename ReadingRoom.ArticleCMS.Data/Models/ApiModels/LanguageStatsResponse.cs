namespace ReadingRoom.ArticleCMS.Data.Models.ApiModels;

/// <summary>
/// The article numbers for one language: how many articles are published in it and the newest ones.
/// </summary>
public sealed class LanguageStatsResponse
{
    public string Culture { get; set; } = string.Empty;

    public string CultureName { get; set; } = string.Empty;

    public int TotalArticles { get; set; }

    public List<LatestArticleResponse> LatestArticles { get; set; } = new List<LatestArticleResponse>();
}
