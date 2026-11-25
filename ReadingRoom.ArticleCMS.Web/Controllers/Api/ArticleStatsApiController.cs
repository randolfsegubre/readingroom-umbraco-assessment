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

    private readonly IPublishedContentQuery _contentQuery;
    private readonly ILanguageService _languageService;
    private readonly IVariationContextAccessor _variationContextAccessor;

    public ArticleStatsApiController(
        IPublishedContentQuery contentQuery,
        ILanguageService languageService,
        IVariationContextAccessor variationContextAccessor)
    {
        _contentQuery = contentQuery;
        _languageService = languageService;
        _variationContextAccessor = variationContextAccessor;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ArticleStatsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        ArticleStatsResponse response = new ArticleStatsResponse();

        foreach (ILanguage language in await _languageService.GetAllAsync())
        {
            // The Title property reads the language of the variation context, so set it for each language.
            _variationContextAccessor.VariationContext = new VariationContext(language.IsoCode);

            List<Article> articles = new List<Article>();
            foreach (IPublishedContent root in _contentQuery.ContentAtRoot())
            {
                foreach (Article article in root.DescendantsOrSelf<Article>(language.IsoCode))
                {
                    if (article.IsPublished(language.IsoCode))
                    {
                        articles.Add(article);
                    }
                }
            }

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
