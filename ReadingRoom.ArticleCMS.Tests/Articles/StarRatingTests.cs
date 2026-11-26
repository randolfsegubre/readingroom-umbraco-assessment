using ReadingRoom.ArticleCMS.Core.Articles;

namespace ReadingRoom.ArticleCMS.Tests.Articles;

public class StarRatingTests
{
    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(5, true)]
    [InlineData(6, false)]
    [InlineData(-1, false)]
    public void IsRated_IsTrueOnlyFromOneToTheMaximum(int rating, bool expected)
    {
        Assert.Equal(expected, StarRating.IsRated(rating));
    }

    [Theory]
    [InlineData(0, "☆☆☆☆☆")]
    [InlineData(3, "★★★☆☆")]
    [InlineData(5, "★★★★★")]
    [InlineData(9, "★★★★★")]
    [InlineData(-2, "☆☆☆☆☆")]
    public void ToStars_DrawsFilledAndEmptyStars(int rating, string expected)
    {
        Assert.Equal(expected, StarRating.ToStars(rating));
    }
}
