# ReadingRoom.ArticleCMS

An article site built on Umbraco 17 and .NET 10 for the Reading Room Umbraco technical test. Editors write articles in the Umbraco backoffice. The site lists them, shows each one with an estimated reading time, and shows a short list of key takeaways above the body text.

Status: the required parts are done. Optional extras that are not built are listed under Improvements.

## What is in it

- Document types: Article List and Article. Each has a template, a strongly typed model and a Razor view (`Views/ArticleList.cshtml` and `Views/Article.cshtml`). An article has a title, an intro, a body, a publish date and a hero image.
- Reading time: a small service counts the words in the article body, and the article page shows "~N min read".
- Key takeaways: a custom property editor built as an Umbraco 17 Property Editor user interface (UI). Editors add, edit and remove short lines, and the article page shows them as a bulleted list.
- Tests: xUnit tests for the reading time service.

## Requirements

- .NET Software Development Kit (SDK) 10.0.403 or a newer 10.0 release (pinned in `global.json`).
- Visual Studio 2026, or any editor plus the `dotnet` command line.

## Run it

From the repository root:

    dotnet run --project ReadingRoom.ArticleCMS.Web

The site listens on https://localhost:44308 and http://localhost:26160 (both are set in `Properties/launchSettings.json`). The backoffice is at `/umbraco` on either address. If the browser warns about the https certificate, run `dotnet dev-certs https --trust` once.

In Visual Studio, open `ReadingRoom.ArticleCMS.slnx`, make the Web project the startup project, choose the "Web" profile (opens the site) or the "Umbraco" profile (opens the backoffice login) and press F5. Both use the same addresses.

Use only one of the two at a time. They share one port and one database.

Run the tests with:

    dotnet test

## Sign in

The repository includes a ready SQLite database. It holds the content types, the three sample articles and an administrator account, so there is nothing to install. Sign in at `/umbraco` with:

- Email: randolf.segubre@example.test
- Password: Welcome123!

This is a throwaway account for local development only. Do not reuse it anywhere else.

## Try the features

- `/` shows the Article List. Each article links to its own page.
- `/lorem-ipsum-quick-read/` shows about 1 minute and three key takeaways. The medium read shows about 3 minutes and the long read about 4 minutes.
- To change the takeaways, open Content in the backoffice, choose Lorem Ipsum Quick Read and use the Takeaways List field. Save and publish to see the change on the page.
- The reading speed is `ReadingTime:WordsPerMinute` in `appsettings.json` (200 words per minute).

## Project layout

| Project | Purpose |
|---|---|
| ReadingRoom.ArticleCMS.Web | The Umbraco site: startup, views, the composer and the backoffice extension in `App_Plugins` |
| ReadingRoom.ArticleCMS.Core | Plain C# with no Umbraco dependency: the reading time service and its options |
| ReadingRoom.ArticleCMS.Data | Models Builder output: the strongly typed models for the Umbraco document types |
| ReadingRoom.ArticleCMS.Tests | xUnit tests for Core |

Web references Core and Data. Tests references Core. Core references nothing. Umbraco itself is the data store, so there is no separate repository layer.

## Decisions

- Models Builder runs in Development only (`appsettings.Development.json`) and writes its models into the Data project. Production keeps `ModelsMode` set to `Nothing`, so models are compiled and deployed and not generated live.
- The reading time service lives in Core and does not know about Umbraco, so it can be unit tested on its own. It receives the body text as a string. Its settings class, `ReadingTimeOptions`, sits next to it.
- `Composers/ReadingTimeComposer.cs` is the one place that maps `IReadingTimeService` to `ReadingTimeService`. Umbraco runs composers at startup, so this is the composition root. The view receives the service through `@inject`. The service is a singleton because it only holds a read-only words per minute value.
- The reading speed comes from the `ReadingTime` section of `appsettings.json`, bound into a plain `ReadingTimeOptions` object.
- The key takeaways editor is plain JavaScript in `App_Plugins/ReadingRoom.Editors`, registered by `umbraco-package.json`. It reuses Umbraco's Multiple Text String storage (`Umbraco.MultipleTextstring`), so Umbraco handles saving and gives the model a ready `IEnumerable<string>`.
- Package versions live in one place, `Directory.Packages.props`.
- Warnings are errors in Core, Data and Tests. The Web project is exempt because it contains Umbraco template code that is not ours. The rules in `.editorconfig` are checked by the build for those three projects and shown as warnings in the editor for Web.
- The document types and data types are exported with uSync into `ReadingRoom.ArticleCMS.Web/uSync/v17`, so structure changes show up as readable text in a diff.
- The SQLite database and the three hero images in `wwwroot/media` are committed, so a clone runs with content straight away. The database is cleaned of machine-specific rows before each commit.
- `Umbraco:CMS:Imaging:HMACSecretKey` in `appsettings.json` signs image resize addresses. It is not a login. A production deployment must supply its own value, for example through the environment variable `Umbraco__CMS__Imaging__HMACSecretKey`.

## Assumptions

- An average reader reads 200 words per minute. This is a common rule of thumb, not a standard, so it is configurable.
- Reading time counts the body text only. The title and intro are not counted.
- The body is rich text, so the service removes the Hypertext Markup Language (HTML) tags and decodes entities such as `&amp;` before it counts words.
- Reading time is rounded up, so any article with words shows at least 1 minute. An empty body shows no reading time.
- Key takeaways keep the order the editor gave them. Empty lines are not shown on the page.
- The site has one language and no translated content.
- The hero images are simple placeholder images (`hero-blue.png`, `hero-green.png`, `hero-orange.png`).
- Local development on Windows with SQLite. No other database was tried.

## Trade-offs

- The reading speed is in `appsettings.json`, so changing it needs a new deployment, and editors cannot change it. I chose this because it keeps the service simple and free of Umbraco code. An editor-managed settings page is the next step.
- The editor is plain JavaScript with no build step. That means no extra tooling and very little code, but it has no type checking, and Umbraco's own editors are written with the Lit library.
- Reusing Umbraco's Multiple Text String storage means less code. The cost is that the lines are saved as one text separated by line breaks, so a takeaway cannot contain a line break.
- Committing the database and media makes the project easy to run. The cost is binary files in Git and a cleaning step before each commit.
- Models Builder output is committed in the Data project, so the build does not depend on a running site. The cost is generated files in the history.

## Improvements

- Move the reading speed into a settings document in the backoffice. The service would then read the value on every call, and `appsettings.json` would stay as the fallback.
- Pass editor settings such as minimum and maximum number of lines into the takeaways editor. Umbraco's storage already supports them, and the editor does not use them yet.
- Add drag to reorder for the takeaways.
- Add a newsletter sign-up form handled by a SurfaceController.
- Add a dashboard that uses the Umbraco Management API.
- Add translated content and vary by culture.
- Add a build and test workflow on GitHub.
- Write down, and test, how to rebuild the database from the uSync files.
