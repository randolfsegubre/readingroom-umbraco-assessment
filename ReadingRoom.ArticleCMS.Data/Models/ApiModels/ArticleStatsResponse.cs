namespace ReadingRoom.ArticleCMS.Data.Models.ApiModels;

/// <summary>
/// The numbers the dashboard shows: how many articles are published and the newest ones.
/// </summary>
public sealed class ArticleStatsResponse
{
    public int TotalArticles { get; set; }

    public List<LatestArticleResponse> LatestArticles { get; set; } = new List<LatestArticleResponse>();
}
