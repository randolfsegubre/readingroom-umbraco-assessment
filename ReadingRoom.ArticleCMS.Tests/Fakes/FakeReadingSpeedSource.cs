using ReadingRoom.ArticleCMS.Core.ReadingTime;

namespace ReadingRoom.ArticleCMS.Tests.Fakes;

public sealed class FakeReadingSpeedSource : IReadingSpeedSource
{
    private readonly int? _wordsPerMinute;

    public FakeReadingSpeedSource(int? wordsPerMinute)
    {
        _wordsPerMinute = wordsPerMinute;
    }

    public int? GetWordsPerMinute()
    {
        return _wordsPerMinute;
    }
}
