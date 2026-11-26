using ReadingRoom.ArticleCMS.Core.Constants;

namespace ReadingRoom.ArticleCMS.Core.Articles;

/// <summary>
/// The reading levels of the site. The values match the ones in the backoffice editor, reading-level.js.
/// </summary>
public static class ReadingLevels
{
    private static readonly ReadingLevel[] All =
    {
        new ReadingLevel("beginner", DictionaryKeys.ArticleLevelBeginner, "Beginner"),
        new ReadingLevel("intermediate", DictionaryKeys.ArticleLevelIntermediate, "Intermediate"),
        new ReadingLevel("advanced", DictionaryKeys.ArticleLevelAdvanced, "Advanced")
    };

    /// <summary>
    /// Returns the level with the saved value, or null when nothing or an unknown value was saved.
    /// </summary>
    public static ReadingLevel? Find(string? value)
    {
        foreach (ReadingLevel level in All)
        {
            if (string.Equals(level.Value, value, StringComparison.OrdinalIgnoreCase))
            {
                return level;
            }
        }

        return null;
    }
}
