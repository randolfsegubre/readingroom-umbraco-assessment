using ReadingRoom.ArticleCMS.Core.ReadingTime;
using ReadingRoom.ArticleCMS.Tests.Fakes;

namespace ReadingRoom.ArticleCMS.Tests.ReadingTime;

public class ReadingTimeServiceTests
{
    private readonly ReadingTimeService _service = new ReadingTimeService(new ReadingTimeOptions(), new FakeReadingSpeedSource(null));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("<p></p><br />")]
    public void GetMinutes_ReturnsZero_WhenThereAreNoWords(string? html)
    {
        Assert.Equal(0, _service.GetMinutes(html));
    }

    [Fact]
    public void GetMinutes_ReturnsOne_ForASingleWord()
    {
        Assert.Equal(1, _service.GetMinutes("hello"));
    }

    [Theory]
    [InlineData(200, 1)]
    [InlineData(201, 2)]
    [InlineData(400, 2)]
    [InlineData(401, 3)]
    public void GetMinutes_RoundsUp_AtTheWordBoundaries(int wordCount, int expectedMinutes)
    {
        Assert.Equal(expectedMinutes, _service.GetMinutes(BuildText(wordCount)));
    }

    [Fact]
    public void GetMinutes_DoesNotCountHtmlTagsOrAttributes()
    {
        string html = string.Concat(Enumerable.Repeat("<span class=\"a b c\">word</span> ", 200));

        Assert.Equal(1, _service.GetMinutes(html));
    }

    [Fact]
    public void GetMinutes_CountsWordsOnBothSidesOfAParagraphBreak()
    {
        string html = "<p>" + BuildText(100) + "</p><p>" + BuildText(101) + "</p>";

        Assert.Equal(2, _service.GetMinutes(html));
    }

    [Fact]
    public void GetMinutes_TreatsANonBreakingSpaceAsAWordSeparator()
    {
        string html = string.Join("&nbsp;", Enumerable.Repeat("word", 201));

        Assert.Equal(2, _service.GetMinutes(html));
    }

    [Fact]
    public void GetMinutes_UsesTheConfiguredReadingSpeed()
    {
        ReadingTimeService slowReader = new ReadingTimeService(new ReadingTimeOptions { WordsPerMinute = 100 }, new FakeReadingSpeedSource(null));

        Assert.Equal(2, slowReader.GetMinutes(BuildText(101)));
    }

    [Fact]
    public void GetMinutes_UsesTheEditorReadingSpeed_WhenOneIsSet()
    {
        ReadingTimeService service = new ReadingTimeService(new ReadingTimeOptions(), new FakeReadingSpeedSource(100));

        Assert.Equal(2, service.GetMinutes(BuildText(101)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData(-5)]
    public void GetMinutes_UsesTheDefaultReadingSpeed_WhenTheEditorSpeedIsNotSetOrNotPositive(int? editorWordsPerMinute)
    {
        ReadingTimeService service = new ReadingTimeService(new ReadingTimeOptions(), new FakeReadingSpeedSource(editorWordsPerMinute));

        Assert.Equal(1, service.GetMinutes(BuildText(200)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Constructor_Throws_WhenWordsPerMinuteIsNotPositive(int wordsPerMinute)
    {
        ReadingTimeOptions options = new ReadingTimeOptions { WordsPerMinute = wordsPerMinute };

        Assert.Throws<ArgumentOutOfRangeException>(() => new ReadingTimeService(options, new FakeReadingSpeedSource(null)));
    }

    [Fact]
    public void Constructor_Throws_WhenOptionsAreMissing()
    {
        Assert.Throws<ArgumentNullException>(() => new ReadingTimeService(null!, new FakeReadingSpeedSource(null)));
    }

    [Fact]
    public void Constructor_Throws_WhenTheSpeedSourceIsMissing()
    {
        Assert.Throws<ArgumentNullException>(() => new ReadingTimeService(new ReadingTimeOptions(), null!));
    }

    private static string BuildText(int wordCount)
    {
        return string.Join(" ", Enumerable.Repeat("word", wordCount));
    }
}
