namespace ReadingRoom.ArticleCMS.Core.ReadingTime;


// Settings for the reading time estimate. Bound from the "ReadingTime" section of appsettings.json.
public class ReadingTimeOptions
{
    /// <summary>
    /// Average silent reading speed in words per minute. Must be greater than zero.
    /// </summary>
    public int WordsPerMinute { get; set; } = 200;
}
