namespace ReadingRoom.ArticleCMS.Core.Articles;

/// <summary>
/// The rating an editor gives an article, from 1 to <see cref="Maximum"/> stars. Zero means not rated.
/// </summary>
public static class StarRating
{
    /// <summary>
    /// The highest rating. It matches the maximum of the Star Rating data type.
    /// </summary>
    public const int Maximum = 5;

    private const char FullStar = '★';
    private const char EmptyStar = '☆';

    public static bool IsRated(int rating)
    {
        return rating >= 1 && rating <= Maximum;
    }

    /// <summary>
    /// Draws the rating as filled and empty stars. A value outside the range is moved to the nearest end.
    /// </summary>
    public static string ToStars(int rating)
    {
        int filled = Math.Clamp(rating, 0, Maximum);

        return new string(FullStar, filled) + new string(EmptyStar, Maximum - filled);
    }
}
