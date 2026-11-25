using NPoco;
using ReadingRoom.ArticleCMS.Core.Newsletter;
using ReadingRoom.ArticleCMS.Web.Migrations;
using Umbraco.Cms.Infrastructure.Scoping;

namespace ReadingRoom.ArticleCMS.Web.Adapters;

/// <summary>
/// Saves newsletter sign-ups in a table of the Umbraco database.
/// </summary>
public sealed class DatabaseNewsletterSignUpStore : INewsletterSignUpStore
{
    private readonly IScopeProvider _scopeProvider;

    public DatabaseNewsletterSignUpStore(IScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider;
    }

    public async Task AddAsync(string emailAddress, string culture)
    {
        string email = EmailAddressText.Normalize(emailAddress);

        using IScope scope = _scopeProvider.CreateScope();

        // the value is passed as a parameter, never joined into the query text.
        int saved = await scope.Database.ExecuteScalarAsync<int>(
            new Sql($"SELECT COUNT(*) FROM {NewsletterSignUpRow.Table} WHERE email = @0", email));

        if (saved == 0)
        {
            await scope.Database.InsertAsync(new NewsletterSignUpRow
            {
                Email = email,
                Culture = culture,
                CreatedUtc = DateTime.UtcNow
            });
        }

        scope.Complete();
    }
}
