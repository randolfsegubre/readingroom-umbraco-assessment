using NPoco;
using ReadingRoom.ArticleCMS.Core.Newsletter;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace ReadingRoom.ArticleCMS.Web.Migrations;

/// <summary>
/// One row of the table of newsletter sign-ups.
/// </summary>
[TableName(Table)]
[PrimaryKey("id", AutoIncrement = true)]
[ExplicitColumns]
public sealed class NewsletterSignUpRow
{
    public const string Table = "readingRoomNewsletterSignUp";

    [Column("id")]
    [PrimaryKeyColumn(AutoIncrement = true)]
    public int Id { get; set; }

    [Column("email")]
    [Length(EmailAddressText.MaximumLength)]
    [Index(IndexTypes.UniqueNonClustered, Name = "IX_readingRoomNewsletterSignUp_email")]
    public string Email { get; set; } = string.Empty;

    [Column("culture")]
    [Length(20)]
    public string Culture { get; set; } = string.Empty;

    [Column("createdUtc")]
    public DateTime CreatedUtc { get; set; }
}
