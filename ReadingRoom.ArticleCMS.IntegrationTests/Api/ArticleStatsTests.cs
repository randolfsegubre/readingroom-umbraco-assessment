using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ReadingRoom.ArticleCMS.Data.Models.ApiModels;
using ReadingRoom.ArticleCMS.IntegrationTests.Fixtures;
using ReadingRoom.ArticleCMS.Web.Controllers.Api;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Web;

namespace ReadingRoom.ArticleCMS.IntegrationTests.Api;

[Collection(SiteTestGroup.Name)]
public class ArticleStatsTests
{
    private readonly SiteFixture _site;

    public ArticleStatsTests(SiteFixture site)
    {
        _site = site;
    }

    [Fact]
    public async Task GetStats_CountsTheArticlesOfEachLanguage_NewestFirst()
    {
        // the controller is called directly, so the test does not need a signed in backoffice user.
        using IServiceScope scope = _site.Services.CreateScope();
        using UmbracoContextReference context = scope.ServiceProvider
            .GetRequiredService<IUmbracoContextFactory>()
            .EnsureUmbracoContext();
        ArticleStatsApiController controller = ActivatorUtilities.CreateInstance<ArticleStatsApiController>(scope.ServiceProvider);

        OkObjectResult result = Assert.IsType<OkObjectResult>(await controller.GetStats());
        ArticleStatsResponse stats = Assert.IsType<ArticleStatsResponse>(result.Value);

        List<string> cultures = stats.Languages.Select(language => language.Culture).Order(StringComparer.Ordinal).ToList();
        Assert.Equal(new List<string> { "en-US", "es" }, cultures);
        Assert.All(stats.Languages, language => Assert.Equal(3, language.TotalArticles));

        LanguageStatsResponse spanish = stats.Languages.Single(language => language.Culture == "es");
        Assert.Equal("Lorem Ipsum Lectura Larga", spanish.LatestArticles[0].Title);
    }
}
