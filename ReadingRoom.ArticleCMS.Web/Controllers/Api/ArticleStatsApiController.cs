using ReadingRoom.ArticleCMS.Web.Adapters;
using Umbraco.Cms.Core.Models;

namespace ReadingRoom.ArticleCMS.Web.Controllers.Api;
/// <summary>
/// A Management API endpoint for the backoffice dashboard. Only signed in backoffice users
/// who can open the Content section may call it.
/// </summary>
[VersionedApiBackOfficeRoute(ReadingRoomApi.StatsRoute)]
[ApiExplorerSettings(GroupName = ReadingRoomApi.GroupName)]
[Authorize(Policy = AuthorizationPolicies.SectionAccessContent)]
public sealed class ArticleStatsApiController : ManagementApiControllerBase
{
    private const int LatestArticleCount = 5;

    private readonly IPublishedArticleReader _articleReader;
    private readonly ILanguageService _languageService;

    public ArticleStatsApiController(IPublishedArticleReader articleReader, ILanguageService languageService)
    {
        _articleReader = articleReader;
        _languageService = languageService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ArticleStatsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        ArticleStatsResponse response = new ArticleStatsResponse();

        foreach (ILanguage language in await _languageService.GetAllAsync())
        {
            List<Article> articles = _articleReader.GetPublished(language.IsoCode);

            LanguageStatsResponse languageStats = new LanguageStatsResponse
            {
                Culture = language.IsoCode,
                CultureName = language.CultureName,
                TotalArticles = articles.Count
            };

            foreach (Article article in articles.OrderByDescending(article => article.PublishDate).Take(LatestArticleCount))
            {
                languageStats.LatestArticles.Add(new LatestArticleResponse
                {
                    Title = article.Title ?? string.Empty,
                    PublishDate = article.PublishDate
                });
            }

            response.Languages.Add(languageStats);
        }

        return Ok(response);
    }
}
