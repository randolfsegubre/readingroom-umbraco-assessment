using Microsoft.AspNetCore.Mvc;
using ReadingRoom.ArticleCMS.Data.Models.ViewModels;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;

namespace ReadingRoom.ArticleCMS.Web.Controllers;

/// <summary>
/// Handles the newsletter sign-up form that is shown on every page.
/// </summary>
public sealed class NewsletterSurfaceController : SurfaceController
{
    public const string MessageKey = "NewsletterMessage";

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
        TempData[MessageKey] = "Thank you. You are on the list.";
        return RedirectToCurrentUmbracoPage();
    }
}
