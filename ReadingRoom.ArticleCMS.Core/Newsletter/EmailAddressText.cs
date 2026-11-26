namespace ReadingRoom.ArticleCMS.Core.Newsletter;

public static class EmailAddressText
{
    /// <summary>
    /// The longest address that is accepted.
    /// </summary>
    public const int MaximumLength = 254;

    /// <summary>
    /// Removes spaces around the address and writes it in lower case, so one address is saved only once.
    /// </summary>
    public static string Normalize(string emailAddress)
    {
        ArgumentNullException.ThrowIfNull(emailAddress);

        return emailAddress.Trim().ToLowerInvariant();
    }
}
