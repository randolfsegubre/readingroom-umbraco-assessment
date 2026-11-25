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

    public ArticleStatsApiController(IPublishedContentQuery contentQuery)
    {
        _contentQuery = contentQuery;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ArticleStatsResponse), StatusCodes.Status200OK)]
    public IActionResult GetStats()
    {
        List<Article> articles = new List<Article>();
        foreach (IPublishedContent root in _contentQuery.ContentAtRoot())
        {
            articles.AddRange(root.DescendantsOrSelf<Article>());
        }

        ArticleStatsResponse response = new ArticleStatsResponse
        {
            TotalArticles = articles.Count
        };

        foreach (Article article in articles.OrderByDescending(article => article.PublishDate).Take(LatestArticleCount))
        {
            response.LatestArticles.Add(new LatestArticleResponse
            {
                Title = article.Title ?? string.Empty,
                PublishDate = article.PublishDate
            });
        }

        return Ok(response);
    }
}
