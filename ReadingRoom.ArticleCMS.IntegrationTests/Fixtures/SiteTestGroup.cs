namespace ReadingRoom.ArticleCMS.IntegrationTests.Fixtures;

/// <summary>
/// The tests in this collection share one running site and run one after the other.
/// </summary>
[CollectionDefinition(Name)]
public sealed class SiteTestGroup : ICollectionFixture<SiteFixture>
{
    public const string Name = "Site";
}
