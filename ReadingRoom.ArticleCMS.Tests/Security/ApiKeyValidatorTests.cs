using ReadingRoom.ArticleCMS.Core.Security;

namespace ReadingRoom.ArticleCMS.Tests.Security;

public class ApiKeyValidatorTests
{
    [Fact]
    public void IsValid_ReturnsTrue_WhenTheKeysAreTheSame()
    {
        Assert.True(ApiKeyValidator.IsValid("a-secret-key", "a-secret-key"));
    }

    [Theory]
    [InlineData("another-key")]
    [InlineData("A-SECRET-KEY")]
    [InlineData("a-secret-ke")]
    [InlineData("a-secret-key ")]
    [InlineData("")]
    [InlineData(null)]
    public void IsValid_ReturnsFalse_WhenTheProvidedKeyIsDifferentOrMissing(string? providedKey)
    {
        Assert.False(ApiKeyValidator.IsValid("a-secret-key", providedKey));
    }

    [Theory]
    [InlineData(null, "a-secret-key")]
    [InlineData("", "")]
    [InlineData("   ", "   ")]
    [InlineData(null, null)]
    public void IsValid_ReturnsFalse_WhenNoKeyIsConfigured(string? configuredKey, string? providedKey)
    {
        Assert.False(ApiKeyValidator.IsValid(configuredKey, providedKey));
    }
}
