namespace ReadingRoom.ArticleCMS.Core.ReadingTime;

public class ReadingTimeOptions
{
    /// <summary>
    /// Average silent reading speed in words per minute. Must be greater than zero.
    /// </summary>
    public int WordsPerMinute { get; set; } = 200;
}
