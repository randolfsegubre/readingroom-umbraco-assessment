using System.Globalization;
using ReadingRoom.ArticleCMS.Core.Newsletter;

namespace ReadingRoom.ArticleCMS.Web.Controllers;

/// <summary>
/// Handles the newsletter sign-up form that is shown on every page.
/// </summary>
public sealed class NewsletterSurfaceController : SurfaceController
{
    private readonly ILogger<NewsletterSurfaceController> _logger;
    private readonly INewsletterSignUpStore _signUpStore;

    public NewsletterSurfaceController(
        IUmbracoContextAccessor umbracoContextAccessor,
        IUmbracoDatabaseFactory databaseFactory,
        ServiceContext services,
        AppCaches appCaches,
        IProfilingLogger profilingLogger,
        IPublishedUrlProvider publishedUrlProvider,
        ILogger<NewsletterSurfaceController> logger,
        INewsletterSignUpStore signUpStore)
        : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
    {
        _logger = logger;
        _signUpStore = signUpStore;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Subscribe(NewsletterSignUpViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Show the same page again. ModelState carries the messages and the typed value.
            return CurrentUmbracoPage();
        }

        await _signUpStore.AddAsync(model.Email, CultureInfo.CurrentUICulture.Name);

        // the address is personal data, so it is not written to the log.
        _logger.LogInformation("A newsletter sign-up was saved.");

        // Post/Redirect/Get: the redirect stops a browser refresh from posting the form again.
        TempData[TempDataKeys.NewsletterMessage] = DictionaryKeys.NewsletterThankYou;
        return RedirectToCurrentUmbracoPage();
    }
}
