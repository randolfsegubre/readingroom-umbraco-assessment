using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using ReadingRoom.ArticleCMS.Core.Constants;

namespace ReadingRoom.ArticleCMS.IntegrationTests.Fixtures;

/// <summary>
/// Starts the real site once, in memory, on a throwaway copy of the committed database.
/// </summary>
public sealed class SiteFixture : IDisposable
{
    private const string DatabaseFileName = "Umbraco.sqlite.db";

    private readonly string _databaseFolder;
    private readonly WebApplicationFactory<Program> _factory;

    public SiteFixture()
    {
        _databaseFolder = Directory.CreateTempSubdirectory("readingroom-tests-").FullName;
        string database = Path.Combine(_databaseFolder, DatabaseFileName);
        File.Copy(Path.Combine(FindRepositoryFolder(), "ReadingRoom.ArticleCMS.Web", "umbraco", "Data", DatabaseFileName), database);

        // the site reads its settings while it is built, so they are set as environment variables first.
        Set("ConnectionStrings__umbracoDbDSN", $"Data Source={database};Cache=Shared;Foreign Keys=True;Pooling=True");
        Set("ConnectionStrings__umbracoDbDSN_ProviderName", "Microsoft.Data.Sqlite");
        Set("Umbraco__CMS__Runtime__Mode", "BackofficeDevelopment");
        Set("Umbraco__CMS__Global__UseHttps", "false");
        Set("Umbraco__CMS__DeliveryApi__ApiKey", ApiKey);

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => builder.UseEnvironment("Testing"));
    }

    /// <summary>
    /// A new key for every test run.
    /// </summary>
    public string ApiKey { get; } = Guid.NewGuid().ToString("N");

    public IServiceProvider Services => _factory.Services;

    /// <summary>
    /// A client without the API key.
    /// </summary>
    public HttpClient CreateClient()
    {
        return _factory.CreateClient();
    }

    /// <summary>
    /// A client that sends the API key.
    /// </summary>
    public HttpClient CreateClientWithKey()
    {
        HttpClient client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiKeyAuthentication.HeaderName, ApiKey);

        return client;
    }

    public void Dispose()
    {
        _factory.Dispose();
        SqliteConnection.ClearAllPools();

        try
        {
            Directory.Delete(_databaseFolder, recursive: true);
        }
        catch (IOException)
        {
            // the operating system removes the temporary folder later if a file is still open.
        }
    }

    private static void Set(string name, string value)
    {
        Environment.SetEnvironmentVariable(name, value);
    }

    private static string FindRepositoryFolder()
    {
        DirectoryInfo? folder = new DirectoryInfo(AppContext.BaseDirectory);
        while (folder is not null)
        {
            if (File.Exists(Path.Combine(folder.FullName, "ReadingRoom.ArticleCMS.slnx")))
            {
                return folder.FullName;
            }

            folder = folder.Parent;
        }

        throw new InvalidOperationException("The folder with ReadingRoom.ArticleCMS.slnx was not found.");
    }
}
