namespace ReadingRoom.ArticleCMS.Core.ReadingTime;


// Settings for the reading time estimate. Bound from the "ReadingTime" section of appsettings.json.
public class ReadingTimeOptions
{
    /// <summary>
    /// The name of the settings section in appsettings.json.
    /// </summary>
    public const string SectionName = "ReadingTime";

    /// <summary>
    /// Average silent reading speed in words per minute. Must be greater than zero.
    /// </summary>
    public int WordsPerMinute { get; set; } = 200;
}
