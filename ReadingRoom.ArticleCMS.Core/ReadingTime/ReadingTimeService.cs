using System.Net;
using System.Text.RegularExpressions;

namespace ReadingRoom.ArticleCMS.Core.ReadingTime;


public sealed partial class ReadingTimeService : IReadingTimeService
{
    private readonly int _wordsPerMinute;

    /// <summary>
    /// Creates the service from its settings.
    /// </summary>
    /// <param name="options">The reading speed. WordsPerMinute must be greater than zero.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public ReadingTimeService(ReadingTimeOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.WordsPerMinute <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "WordsPerMinute must be greater than zero.");
        }

        _wordsPerMinute = options.WordsPerMinute;
    }


    /// <summary>
    /// Returns the estimated reading time in whole minutes.
    /// </summary>
    /// <param name="html">The article body as HTML. Null or empty text gives 0.</param>
    /// <returns>0 when there are no words; otherwise at least 1, rounded up.</returns>
    public int GetMinutes(string? html)
    {
        // no text means no reading time.
        if (string.IsNullOrWhiteSpace(html))
        {
            return 0;
        }

        // replace every HTML tag with a space, so "one</p><p>two" does not become one word.
        string withoutTags = HtmlTagRegex().Replace(html, " ");

        // turn "&amp;" and "&nbsp;" into real characters, then count the words.
        string text = WebUtility.HtmlDecode(withoutTags);
        int wordCount = WordRegex().Count(text);

        // divide by the reading speed and round up with whole-number arithmetic.
        return (wordCount + _wordsPerMinute - 1) / _wordsPerMinute;
    }

    [GeneratedRegex("<[^>]*>")]
    private static partial Regex HtmlTagRegex();

    [GeneratedRegex(@"\S+")]
    private static partial Regex WordRegex();
}
