namespace ReadingRoom.ArticleCMS.Data.Models.ApiModels;

/// <summary>
/// One article in the list of the articles API.
/// </summary>
public class ArticleSummaryResponse
{
    public Guid Key { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Intro { get; set; } = string.Empty;

    public DateTime PublishDate { get; set; }

    public int ReadingMinutes { get; set; }

    /// <summary>
    /// beginner, intermediate or advanced, or null when the editor did not pick one.
    /// </summary>
    public string? ReadingLevel { get; set; }

    /// <summary>
    /// From 1 to 5 stars, or null when the editor did not rate the article.
    /// </summary>
    public int? EditorRating { get; set; }

    public string Url { get; set; } = string.Empty;
}
