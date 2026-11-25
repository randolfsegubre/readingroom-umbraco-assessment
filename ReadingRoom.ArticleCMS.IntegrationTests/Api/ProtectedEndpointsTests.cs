using System.Net;
using ReadingRoom.ArticleCMS.IntegrationTests.Fixtures;

namespace ReadingRoom.ArticleCMS.IntegrationTests.Api;

[Collection(SiteTestGroup.Name)]
public class ProtectedEndpointsTests
{
    private readonly SiteFixture _site;

    public ProtectedEndpointsTests(SiteFixture site)
    {
        _site = site;
    }

    [Theory]
    [InlineData("/umbraco/delivery/api/v2/content")]
    [InlineData("/umbraco/management/api/v1/reading-room/stats")]
    public async Task TheEndpoint_Returns401_ToAnAnonymousCaller(string url)
    {
        using HttpClient client = _site.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(new Uri(url, UriKind.Relative));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task TheStatsEndpoint_Returns401_EvenWithTheApiKey()
    {
        using HttpClient client = _site.CreateClientWithKey();

        using HttpResponseMessage response = await client.GetAsync(
            new Uri("/umbraco/management/api/v1/reading-room/stats", UriKind.Relative));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
