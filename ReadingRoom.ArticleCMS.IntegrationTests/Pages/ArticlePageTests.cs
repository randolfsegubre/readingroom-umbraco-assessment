using System.Net;
using ReadingRoom.ArticleCMS.IntegrationTests.Fixtures;

namespace ReadingRoom.ArticleCMS.IntegrationTests.Pages;

[Collection(SiteTestGroup.Name)]
public class ArticlePageTests
{
    private readonly SiteFixture _site;

    public ArticlePageTests(SiteFixture site)
    {
        _site = site;
    }

    [Fact]
    public async Task TheEnglishArticle_ShowsTheReadingTimeAndTheTakeaways()
    {
        string html = await GetPageAsync("/en/lorem-ipsum-long-read/");

        Assert.Contains("<html lang=\"en", html, StringComparison.Ordinal);
        Assert.Contains("~4 min read", html, StringComparison.Ordinal);
        Assert.Contains("Key takeaways", html, StringComparison.Ordinal);
        Assert.Contains("Advanced", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task TheSpanishArticle_UsesTheSpanishLabels()
    {
        string html = await GetPageAsync("/es/lectura-larga/");

        Assert.Contains("<html lang=\"es", html, StringComparison.Ordinal);
        Assert.Contains("min de lectura", html, StringComparison.Ordinal);
        Assert.Contains("Puntos clave", html, StringComparison.Ordinal);
        Assert.Contains("Avanzado", html, StringComparison.Ordinal);
    }

    [Fact]
    public async Task TheArticleList_LinksToEveryArticle()
    {
        string html = await GetPageAsync("/en/");

        Assert.Contains("/en/lorem-ipsum-quick-read/", html, StringComparison.Ordinal);
        Assert.Contains("/en/lorem-ipsum-medium-read/", html, StringComparison.Ordinal);
        Assert.Contains("/en/lorem-ipsum-long-read/", html, StringComparison.Ordinal);
    }

    private async Task<string> GetPageAsync(string url)
    {
        using HttpClient client = _site.CreateClient();
        using HttpResponseMessage response = await client.GetAsync(new Uri(url, UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return await response.Content.ReadAsStringAsync();
    }
}
