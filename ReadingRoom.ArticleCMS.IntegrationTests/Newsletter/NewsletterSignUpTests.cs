using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using ReadingRoom.ArticleCMS.IntegrationTests.Fixtures;
using ReadingRoom.ArticleCMS.Web.Migrations;
using Umbraco.Cms.Infrastructure.Scoping;

namespace ReadingRoom.ArticleCMS.IntegrationTests.Newsletter;

[Collection(SiteTestGroup.Name)]
public partial class NewsletterSignUpTests
{
    private static readonly Uri Page = new Uri("/en/", UriKind.Relative);

    private readonly SiteFixture _site;

    public NewsletterSignUpTests(SiteFixture site)
    {
        _site = site;
    }

    [Fact]
    public async Task SendingTheSameAddressTwice_SavesItOnce()
    {
        string email = $"reader-{Guid.NewGuid():N}@example.test";

        string first = await SubscribeAsync(email);
        string second = await SubscribeAsync(email.ToUpperInvariant());

        // the visitor gets the same answer both times, so the form does not tell who is on the list.
        Assert.Contains("Thank you", first, StringComparison.Ordinal);
        Assert.Contains("Thank you", second, StringComparison.Ordinal);
        Assert.Equal(1, CountSaved(email));
    }

    [Fact]
    public async Task SendingATextThatIsNotAnAddress_SavesNothing()
    {
        string answer = await SubscribeAsync("not-an-address");

        Assert.DoesNotContain("Thank you", answer, StringComparison.Ordinal);
        Assert.Equal(0, CountSaved("not-an-address"));
    }

    private async Task<string> SubscribeAsync(string email)
    {
        using HttpClient client = _site.CreateClient();
        string form = await client.GetStringAsync(Page);

        using FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Email"] = email,
            ["__RequestVerificationToken"] = HiddenValue(form, "__RequestVerificationToken"),
            ["ufprt"] = HiddenValue(form, "ufprt")
        });

        using HttpResponseMessage response = await client.PostAsync(Page, content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return await response.Content.ReadAsStringAsync();
    }

    private int CountSaved(string email)
    {
        using IServiceScope services = _site.Services.CreateScope();
        IScopeProvider scopeProvider = services.ServiceProvider.GetRequiredService<IScopeProvider>();

        using IScope scope = scopeProvider.CreateScope(autoComplete: true);

        return scope.Database.ExecuteScalar<int>(
            $"SELECT COUNT(*) FROM {NewsletterSignUpRow.Table} WHERE email = @0",
            email);
    }

    private static string HiddenValue(string html, string name)
    {
        Match input = Regex.Match(html, $"<input[^>]*name=\"{name}\"[^>]*>");
        Assert.True(input.Success, $"The form has no field {name}.");

        return ValueAttribute().Match(input.Value).Groups[1].Value;
    }

    [GeneratedRegex("value=\"([^\"]*)\"")]
    private static partial Regex ValueAttribute();
}
