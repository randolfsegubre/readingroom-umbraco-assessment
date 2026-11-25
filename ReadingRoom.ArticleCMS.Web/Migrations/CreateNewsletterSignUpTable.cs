using Umbraco.Cms.Infrastructure.Migrations;

namespace ReadingRoom.ArticleCMS.Web.Migrations;

/// <summary>
/// Creates the table of newsletter sign-ups.
/// </summary>
public sealed class CreateNewsletterSignUpTable : AsyncMigrationBase
{
    public CreateNewsletterSignUpTable(IMigrationContext context)
        : base(context)
    {
    }

    protected override Task MigrateAsync()
    {
        if (!TableExists(NewsletterSignUpRow.Table))
        {
            Create.Table<NewsletterSignUpRow>().Do();
        }

        return Task.CompletedTask;
    }
}
