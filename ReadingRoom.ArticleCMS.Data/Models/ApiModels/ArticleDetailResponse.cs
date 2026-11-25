namespace ReadingRoom.ArticleCMS.Data.Models.ApiModels;

/// <summary>
/// A single article of the articles API, with its body and key takeaways.
/// </summary>
public sealed class ArticleDetailResponse : ArticleSummaryResponse
{
    public string BodyHtml { get; set; } = string.Empty;

    public List<string> Takeaways { get; set; } = new List<string>();

    public string? HeroImageUrl { get; set; }
}
