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

    public string Url { get; set; } = string.Empty;
}
