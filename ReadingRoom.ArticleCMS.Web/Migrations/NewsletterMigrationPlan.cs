using Umbraco.Cms.Core.Packaging;

namespace ReadingRoom.ArticleCMS.Web.Migrations;

/// <summary>
/// The database changes of the newsletter. Umbraco finds this class and runs the new steps when the site starts.
/// </summary>
public sealed class NewsletterMigrationPlan : PackageMigrationPlan
{
    public NewsletterMigrationPlan()
        : base("ReadingRoom.Newsletter")
    {
    }

    protected override void DefinePlan()
    {
        To<CreateNewsletterSignUpTable>(new Guid("b0f4c7e2-3a51-4d0a-9b1e-6c2d8a4f7e10"));
    }
}
