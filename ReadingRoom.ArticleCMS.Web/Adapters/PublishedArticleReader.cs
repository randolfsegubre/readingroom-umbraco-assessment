namespace ReadingRoom.ArticleCMS.Web.Adapters;

/// <summary>
/// Reads the published articles of one language from the Umbraco content.
/// </summary>
public sealed class PublishedArticleReader : IPublishedArticleReader
{
    private readonly IPublishedContentQuery _contentQuery;
    private readonly IVariationContextAccessor _variationContextAccessor;

    public PublishedArticleReader(IPublishedContentQuery contentQuery, IVariationContextAccessor variationContextAccessor)
    {
        _contentQuery = contentQuery;
        _variationContextAccessor = variationContextAccessor;
    }

    public List<Article> GetPublished(string culture)
    {
        // the typed properties such as Title read the language of the variation context, so set it first.
        _variationContextAccessor.VariationContext = new VariationContext(culture);

        List<Article> articles = new List<Article>();
        foreach (IPublishedContent root in _contentQuery.ContentAtRoot())
        {
            foreach (Article article in root.DescendantsOrSelf<Article>(culture))
            {
                if (article.IsPublished(culture))
                {
                    articles.Add(article);
                }
            }
        }

        return articles;
    }
}
