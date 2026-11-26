using System.Net;
using System.Net.Http.Json;
using ReadingRoom.ArticleCMS.Core.Constants;
using ReadingRoom.ArticleCMS.Data.Models.ApiModels;
using ReadingRoom.ArticleCMS.IntegrationTests.Fixtures;

namespace ReadingRoom.ArticleCMS.IntegrationTests.Api;

[Collection(SiteTestGroup.Name)]
public class ArticlesApiTests
{
    private const string Route = "/" + ReadingRoomApi.ArticlesRoute;

    private readonly SiteFixture _site;

    public ArticlesApiTests(SiteFixture site)
    {
        _site = site;
    }

    [Theory]
    [InlineData(null)]
    [InlineData("a-wrong-key")]
    public async Task GetAll_Returns401_WithoutTheRightKey(string? key)
    {
        using HttpClient client = _site.CreateClient();
        if (key is not null)
        {
            client.DefaultRequestHeaders.Add(ApiKeyAuthentication.HeaderName, key);
        }

        using HttpResponseMessage response = await client.GetAsync(new Uri(Route, UriKind.Relative));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ReturnsThePublishedArticles_NewestFirst()
    {
        ArticleListResponse list = await GetListAsync(Route);

        Assert.Equal(3, list.Total);
        List<DateTime> dates = list.Items.Select(item => item.PublishDate).ToList();
        Assert.Equal(dates.OrderByDescending(date => date), dates);
        Assert.All(list.Items, item => Assert.StartsWith("/en/", item.Url, StringComparison.Ordinal));
        Assert.All(list.Items, item => Assert.True(item.ReadingMinutes > 0));
    }

    [Fact]
    public async Task GetAll_ReturnsTheSpanishArticles_ForTheSpanishCulture()
    {
        ArticleListResponse list = await GetListAsync(Route + "?culture=es");

        Assert.Equal(3, list.Total);
        Assert.All(list.Items, item => Assert.StartsWith("/es/", item.Url, StringComparison.Ordinal));
    }

    [Fact]
    public async Task GetAll_SkipsAndTakes_ButCountsEveryArticle()
    {
        ArticleListResponse all = await GetListAsync(Route);
        ArticleListResponse page = await GetListAsync(Route + "?skip=1&take=1");

        Assert.Equal(all.Total, page.Total);
        ArticleSummaryResponse only = Assert.Single(page.Items);
        Assert.Equal(all.Items[1].Key, only.Key);
    }

    [Theory]
    [InlineData("?take=0")]
    [InlineData("?take=51")]
    [InlineData("?skip=-1")]
    [InlineData("?culture=fr")]
    public async Task GetAll_Returns400_ForAValueThatIsNotAllowed(string query)
    {
        using HttpClient client = _site.CreateClientWithKey();

        using HttpResponseMessage response = await client.GetAsync(new Uri(Route + query, UriKind.Relative));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetByKey_ReturnsTheBodyAndTheTakeaways()
    {
        ArticleListResponse list = await GetListAsync(Route);
        using HttpClient client = _site.CreateClientWithKey();

        ArticleDetailResponse? article = await client.GetFromJsonAsync<ArticleDetailResponse>(
            new Uri($"{Route}/{list.Items[0].Key}", UriKind.Relative));

        Assert.NotNull(article);
        Assert.Contains("<p>", article.BodyHtml, StringComparison.Ordinal);
        Assert.NotEmpty(article.Takeaways);
        Assert.NotNull(article.HeroImageUrl);
        Assert.NotNull(article.ReadingLevel);
        Assert.NotNull(article.EditorRating);
    }

    [Fact]
    public async Task GetByKey_Returns404_ForAnUnknownKey()
    {
        using HttpClient client = _site.CreateClientWithKey();

        using HttpResponseMessage response = await client.GetAsync(new Uri($"{Route}/{Guid.NewGuid()}", UriKind.Relative));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<ArticleListResponse> GetListAsync(string url)
    {
        using HttpClient client = _site.CreateClientWithKey();
        ArticleListResponse? list = await client.GetFromJsonAsync<ArticleListResponse>(new Uri(url, UriKind.Relative));

        Assert.NotNull(list);
        return list;
    }
}
