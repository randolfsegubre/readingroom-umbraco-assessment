using ReadingRoom.ArticleCMS.Core.Newsletter;

namespace ReadingRoom.ArticleCMS.Tests.Newsletter;

public class EmailAddressTextTests
{
    [Theory]
    [InlineData("reader@example.test", "reader@example.test")]
    [InlineData("  Reader@Example.TEST  ", "reader@example.test")]
    public void Normalize_TrimsAndLowersTheAddress(string typed, string expected)
    {
        Assert.Equal(expected, EmailAddressText.Normalize(typed));
    }
}
