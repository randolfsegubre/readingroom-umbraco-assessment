namespace ReadingRoom.ArticleCMS.Core.Newsletter;

public interface INewsletterSignUpStore
{
    /// <summary>
    /// Saves a sign-up. An address that is already saved is not saved again.
    /// </summary>
    /// <param name="emailAddress">The address as the visitor typed it.</param>
    /// <param name="culture">The language of the page, for example en-US or es.</param>
    Task AddAsync(string emailAddress, string culture);
}
