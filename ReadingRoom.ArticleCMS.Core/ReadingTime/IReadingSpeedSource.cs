namespace ReadingRoom.ArticleCMS.Core.ReadingTime;

public interface IReadingSpeedSource
{
    /// <summary>
    /// Returns the reading speed that an editor has set, in words per minute.
    /// </summary>
    /// <returns>Null when no speed is set.</returns>
    int? GetWordsPerMinute();
}
