using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using ReadingRoom.ArticleCMS.Core.Constants;
using ReadingRoom.ArticleCMS.Data.Models.ApiModels;
using ReadingRoom.ArticleCMS.IntegrationTests.Fixtures;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace ReadingRoom.ArticleCMS.IntegrationTests.Adapters;

[Collection(SiteTestGroup.Name)]
public class SiteSettingsReadingSpeedTests
{
    private const string SettingsTypeAlias = "siteSettings";
    private const string WordsPerMinuteAlias = "wordsPerMinute";

    private static readonly string[] AllCultures = { "*" };

    private readonly SiteFixture _site;

    public SiteSettingsReadingSpeedTests(SiteFixture site)
    {
        _site = site;
    }

    [Fact]
    public async Task PublishingAReadingSpeed_ChangesTheReadingTime()
    {
        int before = await GetLongestReadingTimeAsync();

        try
        {
            // half of the default speed in appsettings.json, so the reading time doubles.
            PublishWordsPerMinute(100);

            Assert.Equal(before * 2, await GetLongestReadingTimeAsync());
        }
        finally
        {
            PublishWordsPerMinute(null);
        }

        Assert.Equal(before, await GetLongestReadingTimeAsync());
    }

    private void PublishWordsPerMinute(int? wordsPerMinute)
    {
        using IServiceScope scope = _site.Services.CreateScope();
        IContentService contentService = scope.ServiceProvider.GetRequiredService<IContentService>();

        IContent settings = contentService.GetRootContent().Single(content => content.ContentType.Alias == SettingsTypeAlias);
        settings.SetValue(WordsPerMinuteAlias, wordsPerMinute);
        contentService.Save(settings);

        Assert.True(contentService.Publish(settings, AllCultures).Success);
    }

    private async Task<int> GetLongestReadingTimeAsync()
    {
        using HttpClient client = _site.CreateClientWithKey();
        ArticleListResponse? list = await client.GetFromJsonAsync<ArticleListResponse>(
            new Uri("/" + ReadingRoomApi.ArticlesRoute, UriKind.Relative));

        Assert.NotNull(list);
        return list.Items.Max(item => item.ReadingMinutes);
    }
}
