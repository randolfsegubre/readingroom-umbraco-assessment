namespace ReadingRoom.ArticleCMS.Data.Models.ApiModels;

/// <summary>
/// One page of the articles API. Total is the number of articles in the requested language.
/// </summary>
public sealed class ArticleListResponse
{
    public int Total { get; set; }

    public List<ArticleSummaryResponse> Items { get; set; } = new List<ArticleSummaryResponse>();
}
