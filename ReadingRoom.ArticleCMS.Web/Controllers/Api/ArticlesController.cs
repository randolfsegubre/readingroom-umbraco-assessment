using ReadingRoom.ArticleCMS.Core.Articles;
using ReadingRoom.ArticleCMS.Core.Takeaways;
using ReadingRoom.ArticleCMS.Web.Adapters;
using ReadingRoom.ArticleCMS.Web.Authentication;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Core.Models;

namespace ReadingRoom.ArticleCMS.Web.Controllers.Api;

/// <summary>
/// A read-only API for the published articles. Every request needs the Api-Key header.
/// </summary>
[ApiController]
[Route(ReadingRoomApi.ArticlesRoute)]
[MapToApi(ReadingRoomApi.ArticlesDocumentName)]
[ApiExplorerSettings(GroupName = ReadingRoomApi.ArticlesGroupName)]
[ServiceFilter(typeof(ApiKeyAuthorizationFilter))]
[Produces("application/json")]
public sealed class ArticlesController : ControllerBase
{
    private readonly IPublishedArticleReader _articleReader;
    private readonly IReadingTimeService _readingTimeService;
    private readonly ILanguageService _languageService;

    public ArticlesController(
        IPublishedArticleReader articleReader,
        IReadingTimeService readingTimeService,
        ILanguageService languageService)
    {
        _articleReader = articleReader;
        _readingTimeService = readingTimeService;
        _languageService = languageService;
    }

    /// <summary>
    /// Lists the published articles of one language, newest first.
    /// </summary>
    /// <param name="culture">The language, for example en-US or es. The default language is used when it is left out.</param>
    /// <param name="skip">How many articles to skip. Zero or more.</param>
    /// <param name="take">How many articles to return, from 1 to 50.</param>
    [HttpGet]
    [ProducesResponseType(typeof(ArticleListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? culture,
        [FromQuery] int skip = 0,
        [FromQuery] int take = ReadingRoomApi.DefaultPageSize)
    {
        if (skip < 0)
        {
            ModelState.AddModelError(nameof(skip), "skip must be zero or more.");
        }

        if (take < 1 || take > ReadingRoomApi.MaximumPageSize)
        {
            ModelState.AddModelError(nameof(take), $"take must be from 1 to {ReadingRoomApi.MaximumPageSize}.");
        }

        string? language = await FindLanguageAsync(culture);
        if (language is null)
        {
            ModelState.AddModelError(nameof(culture), "This site has no language with that code.");
        }

        if (!ModelState.IsValid || language is null)
        {
            return ValidationProblem(ModelState);
        }

        List<Article> articles = _articleReader.GetPublished(language);
        ArticleListResponse response = new ArticleListResponse
        {
            Total = articles.Count
        };

        foreach (Article article in articles.OrderByDescending(article => article.PublishDate).Skip(skip).Take(take))
        {
            ArticleSummaryResponse summary = new ArticleSummaryResponse();
            Fill(summary, article, language);
            response.Items.Add(summary);
        }

        return Ok(response);
    }

    /// <summary>
    /// Returns one published article with its body and key takeaways.
    /// </summary>
    /// <param name="key">The unique identifier of the article.</param>
    /// <param name="culture">The language, for example en-US or es. The default language is used when it is left out.</param>
    [HttpGet("{key:guid}")]
    [ProducesResponseType(typeof(ArticleDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByKey(Guid key, [FromQuery] string? culture)
    {
        string? language = await FindLanguageAsync(culture);
        if (language is null)
        {
            ModelState.AddModelError(nameof(culture), "This site has no language with that code.");
            return ValidationProblem(ModelState);
        }

        Article? article = _articleReader.GetPublished(language).FirstOrDefault(candidate => candidate.Key == key);
        if (article is null)
        {
            return NotFound();
        }

        ArticleDetailResponse detail = new ArticleDetailResponse
        {
            BodyHtml = article.Body?.ToHtmlString() ?? string.Empty,
            Takeaways = TakeawayLines.WithoutBlanks(article.TakeawaysList),
            HeroImageUrl = article.HeroImage?.Url()
        };
        Fill(detail, article, language);

        return Ok(detail);
    }

    private void Fill(ArticleSummaryResponse target, Article article, string culture)
    {
        target.Key = article.Key;
        target.Title = article.Title ?? string.Empty;
        target.Intro = article.Intro ?? string.Empty;
        target.PublishDate = article.PublishDate;
        target.ReadingMinutes = _readingTimeService.GetMinutes(article.Body?.ToHtmlString());
        target.ReadingLevel = ReadingLevels.Find(article.ReadingLevel)?.Value;
        target.EditorRating = StarRating.IsRated(article.EditorRating) ? article.EditorRating : null;
        target.Url = article.Url(culture);
    }

    /// <summary>
    /// Returns the code of a language of the site, the default one when none is asked for, or null when unknown.
    /// </summary>
    private async Task<string?> FindLanguageAsync(string? culture)
    {
        if (string.IsNullOrWhiteSpace(culture))
        {
            return await _languageService.GetDefaultIsoCodeAsync();
        }

        foreach (ILanguage language in await _languageService.GetAllAsync())
        {
            if (string.Equals(language.IsoCode, culture, StringComparison.OrdinalIgnoreCase))
            {
                return language.IsoCode;
            }
        }

        return null;
    }
}
