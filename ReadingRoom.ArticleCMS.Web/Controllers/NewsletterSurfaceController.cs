namespace ReadingRoom.ArticleCMS.Web.Controllers;

/// <summary>
/// Handles the newsletter sign-up form that is shown on every page.
/// </summary>
public sealed class NewsletterSurfaceController : SurfaceController
{
    private readonly ILogger<NewsletterSurfaceController> _logger;

    public NewsletterSurfaceController(
        IUmbracoContextAccessor umbracoContextAccessor,
        IUmbracoDatabaseFactory databaseFactory,
        ServiceContext services,
        AppCaches appCaches,
        IProfilingLogger profilingLogger,
        IPublishedUrlProvider publishedUrlProvider,
        ILogger<NewsletterSurfaceController> logger)
        : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
    {
        _logger = logger;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Subscribe(NewsletterSignUpViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Show the same page again. ModelState carries the messages and the typed value.
            return CurrentUmbracoPage();
        }

        _logger.LogInformation("A newsletter sign-up was received.");

        // Post/Redirect/Get: the redirect stops a browser refresh from posting the form again.
        TempData[TempDataKeys.NewsletterMessage] = DictionaryKeys.NewsletterThankYou;
        return RedirectToCurrentUmbracoPage();
    }
}
