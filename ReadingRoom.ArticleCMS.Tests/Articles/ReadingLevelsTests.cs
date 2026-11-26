using ReadingRoom.ArticleCMS.Core.Articles;

namespace ReadingRoom.ArticleCMS.Tests.Articles;

public class ReadingLevelsTests
{
    [Theory]
    [InlineData("beginner", "Beginner")]
    [InlineData("Intermediate", "Intermediate")]
    [InlineData("advanced", "Advanced")]
    public void Find_ReturnsTheLevel_ForASavedValue(string value, string expectedName)
    {
        ReadingLevel? level = ReadingLevels.Find(value);

        Assert.NotNull(level);
        Assert.Equal(expectedName, level.EnglishName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("expert")]
    public void Find_ReturnsNull_WhenNothingOrAnUnknownValueWasSaved(string? value)
    {
        Assert.Null(ReadingLevels.Find(value));
    }
}
