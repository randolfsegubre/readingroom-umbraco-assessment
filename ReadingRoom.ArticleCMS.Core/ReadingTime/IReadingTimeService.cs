namespace ReadingRoom.ArticleCMS.Core.ReadingTime;

public interface IReadingTimeService
{
    /// <summary>
    /// Returns the estimated reading time in whole minutes.
    /// </summary>
    /// <param name="html">The article body as HTML. Null or empty text gives 0.</param>
    /// <returns>0 when there are no words; otherwise at least 1, rounded up.</returns>
    int GetMinutes(string? html);
}
