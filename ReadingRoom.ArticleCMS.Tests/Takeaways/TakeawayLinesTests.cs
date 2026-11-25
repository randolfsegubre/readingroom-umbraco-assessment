using ReadingRoom.ArticleCMS.Core.Takeaways;

namespace ReadingRoom.ArticleCMS.Tests.Takeaways;

public class TakeawayLinesTests
{
    [Fact]
    public void WithoutBlanks_RemovesEmptyLinesAndKeepsTheOrder()
    {
        string[] lines = new[] { "first", "", "second", "   ", "third" };

        List<string> expected = new List<string> { "first", "second", "third" };

        Assert.Equal(expected, TakeawayLines.WithoutBlanks(lines));
    }

    [Fact]
    public void WithoutBlanks_ReturnsAnEmptyList_WhenThereAreNoLines()
    {
        Assert.Empty(TakeawayLines.WithoutBlanks(null));
    }
}
