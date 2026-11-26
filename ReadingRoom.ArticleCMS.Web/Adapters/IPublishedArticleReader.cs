namespace ReadingRoom.ArticleCMS.Web.Adapters;

public interface IPublishedArticleReader
{
    /// <summary>
    /// Returns the articles that are published in the given language.
    /// </summary>
    /// <param name="culture">The language, for example en-US or es.</param>
    List<Article> GetPublished(string culture);
}
