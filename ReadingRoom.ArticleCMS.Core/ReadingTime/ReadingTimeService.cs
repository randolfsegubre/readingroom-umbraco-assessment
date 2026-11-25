using System.Net;
using System.Text.RegularExpressions;

namespace ReadingRoom.ArticleCMS.Core.ReadingTime;


public sealed partial class ReadingTimeService : IReadingTimeService
{
    private readonly int _defaultWordsPerMinute;
    private readonly IReadingSpeedSource _speedSource;

    /// <summary>
    /// Creates the service from its settings.
    /// </summary>
    /// <param name="options">The default reading speed. WordsPerMinute must be greater than zero.</param>
    /// <param name="speedSource">Gives the reading speed that an editor has set, when there is one.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public ReadingTimeService(ReadingTimeOptions options, IReadingSpeedSource speedSource)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(speedSource);

        if (options.WordsPerMinute <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "WordsPerMinute must be greater than zero.");
        }

        _defaultWordsPerMinute = options.WordsPerMinute;
        _speedSource = speedSource;
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
        int wordsPerMinute = GetWordsPerMinute();
        return (wordCount + wordsPerMinute - 1) / wordsPerMinute;
    }

    private int GetWordsPerMinute()
    {
        // use the speed that an editor has set, when there is one.
        int? editorWordsPerMinute = _speedSource.GetWordsPerMinute();
        if (editorWordsPerMinute.HasValue && editorWordsPerMinute.Value > 0)
        {
            return editorWordsPerMinute.Value;
        }

        // otherwise use the default speed from the app settings.
        return _defaultWordsPerMinute;
    }

    [GeneratedRegex("<[^>]*>")]
    private static partial Regex HtmlTagRegex();

    [GeneratedRegex(@"\S+")]
    private static partial Regex WordRegex();
}
