namespace ReadingRoom.ArticleCMS.Core.Constants;

public static class ReadingRoomApi
{
    public const string GroupName = "Reading Room";

    public const string StatsRoute = "reading-room/stats";

    /// <summary>
    /// The route of the read-only articles API.
    /// </summary>
    public const string ArticlesRoute = "api/reading-room/articles";

    /// <summary>
    /// The name of the Swagger document of the articles API.
    /// </summary>
    public const string ArticlesDocumentName = "reading-room";

    public const string ArticlesGroupName = "Articles";

    public const int DefaultPageSize = 10;

    public const int MaximumPageSize = 50;
}
