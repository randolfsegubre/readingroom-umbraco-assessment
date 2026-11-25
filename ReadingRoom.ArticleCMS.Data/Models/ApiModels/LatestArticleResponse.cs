namespace ReadingRoom.ArticleCMS.Data.Models.ApiModels;

/// <summary>
/// One entry in the latest articles list of the dashboard.
/// </summary>
public sealed class LatestArticleResponse
{
    public string Title { get; set; } = string.Empty;

    public DateTime PublishDate { get; set; }
}
