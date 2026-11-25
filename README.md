# ReadingRoom.ArticleCMS

An article site built on Umbraco 17 and .NET 10 for the Reading Room Umbraco technical test. Editors write articles in the Umbraco backoffice, in English and Spanish. The site lists the articles, shows each one with an estimated reading time and a short list of key takeaways, and has a newsletter sign-up form. The backoffice has a custom editor for the takeaways and a dashboard with article numbers for each language.

Status: every required part of the test is done, and so are the four stretch goals (editor options, localization, vary by culture and unit tests). The optional extras that are not built are listed under Improvements.

## What is in it

- Document types: Article List and Article. Each has a template, a strongly typed model and a Razor view (`Views/ArticleList.cshtml` and `Views/Article.cshtml`). An article has a title, an intro, a body, a publish date, a hero image, a list of key takeaways, a reading level and an editor rating.
- Reading time: a small service counts the words in the article body, and the article page shows "~N min read". The reading speed comes from a Site Settings document that editors can change, with `appsettings.json` as the fallback.
- Key takeaways: a custom property editor built as an Umbraco 17 Property Editor User Interface (UI). It is a Lit element registered by `umbraco-package.json`. Editors add, edit, remove and reorder short lines, by dragging the handle or with the up and down buttons. The editor reads a placeholder, a minimum and a maximum from its Data Type, says why when the number of lines is outside the limits, and its text is translated through English and Spanish localization files.
- Dashboard: an "Article stats" tab in the Content section. It calls a Management Application Programming Interface (API) endpoint and shows, for each language, the number of published articles and the five newest titles.
- Reading level and star rating: two more custom editors. The reading level is a drop down list (beginner, intermediate or advanced). The star rating is a row of one to five stars with a Clear button. The article page shows both next to the reading time, in the language of the page, and the articles API returns them.
- Newsletter form: a SurfaceController receives the form, shows validation messages, and redirects back to the page with a thank-you message. It does not store the email address.
- Two languages: English (the default) and Spanish. The title, intro, takeaways and body vary by culture. The publish date, the hero image, the reading level and the editor rating are shared. The pages are served under `/en/` and `/es/`, with a language switcher, labels from Umbraco dictionary items and a matching `lang` attribute on the page.
- Application Programming Interfaces: the Umbraco Content Delivery API and a small read-only articles API, both behind an API key and both listed in Swagger.
- Tests: xUnit tests for the reading time service, the API key check, the takeaway lines, the reading levels and the star rating. Integration tests start the real site and call the pages, the APIs and the reading speed setting. A GitHub Actions workflow builds and tests each pull request.

## Quick start

You need the .NET Software Development Kit (SDK) 10.0.403 or a newer 10.0 release (pinned in `global.json`) and an internet connection for the first start. No database server and no Node.js are needed. Visual Studio 2026 is optional.

1. Get the code. Clone the repository with the address you were given, or unzip the zip file. Then open a terminal in the repository folder, the one that holds `ReadingRoom.ArticleCMS.slnx`.
2. Start the site with `dotnet run --project ReadingRoom.ArticleCMS.Web`. The first start downloads the NuGet packages, so it can take a minute or two. Wait for the log line `Application started`. The command uses the Development settings from `Properties/launchSettings.json`.
3. Open https://localhost:44308 in a browser. The same site is also at http://localhost:26160. If the browser warns about the https certificate, run `dotnet dev-certs https --trust` once, or use the http address.
4. For the backoffice, open https://localhost:44308/umbraco and sign in with the account under Sign in below.
5. Stop the site with Ctrl+C in the terminal.

## Sign in

The repository includes a ready SQLite database. It holds the content types, the three sample articles in both languages, the dictionary items, the Site Settings node and an administrator account, so there is nothing to install. Sign in at `/umbraco` with:

- Email: randolf.segubre@example.test
- Password: Welcome123!

This is a throwaway account for local development only. Do not reuse it anywhere else.

## What you should see

- The home page lists three articles. The header has a language switcher for English and Spanish.
- An article page shows a "~N min read" badge, the key takeaways and a newsletter form.
- The backoffice Content section holds the Articles node and the Settings node, and has an "Article stats" tab.

## If something goes wrong

- The site does not start and complains about the .NET SDK: install the version that `global.json` asks for.
- The site does not start and complains about an https certificate: run `dotnet dev-certs https --trust`, then start it again.
- The site does not start and says an address is already in use: another copy is running, for example in Visual Studio. Stop it first. Only one copy can use the ports and the database file at a time.
- You want the original content back: starting the site and signing in writes to the SQLite file in `ReadingRoom.ArticleCMS.Web/umbraco/Data`, and saving in the backoffice rewrites files under `ReadingRoom.ArticleCMS.Web/uSync`. Stop the site and delete `Umbraco.sqlite.db-wal` and `Umbraco.sqlite.db-shm` in that folder if they exist, because they hold changes that belong to the old copy of the database. Then run `git restore ReadingRoom.ArticleCMS.Web/umbraco/Data/Umbraco.sqlite.db ReadingRoom.ArticleCMS.Web/uSync`. With a zip file, unzip it again instead.

## Visual Studio and tests

In Visual Studio 2026, open `ReadingRoom.ArticleCMS.slnx`, make the Web project the startup project, choose the "Web" profile (opens the site) or the "Umbraco" profile (opens the backoffice login) and press F5. Both use the same addresses. Use only one of the two at a time. They share one port and one database.

Run the tests with `dotnet test` from the repository folder. Stop the site first, because a running site locks the files the build writes.

The integration tests start the site in memory on a temporary copy of the committed database, so they do not change your database. They expect the committed content. If you edited content, restore the database first (see above). They take about half a minute.

The workflow in `.github/workflows/ci.yml` runs the same build and tests on Windows for every pull request and every push to `main`.

This was tried on Windows with Visual Studio 2026 and the `dotnet` command line. macOS and Linux were not tried.

## Content Delivery API

Umbraco's Content Delivery Application Programming Interface (API) is switched on and needs a key. Send the key in the `Api-Key` request header. A request without it, or with a wrong key, gets the status 401.

- The development key is `Umbraco:CMS:DeliveryApi:ApiKey` in `appsettings.Development.json`. It is for local use only. A production deployment must supply its own, for example through the environment variable `Umbraco__CMS__DeliveryApi__ApiKey`.
- The list is at `/umbraco/delivery/api/v2/content`. Add the `Accept-Language` header, for example `es`, to get another language.
- The Site Settings document type is excluded, so settings are not published through the API.
- The search index that the API reads is built in the background after the first start, so allow about a minute.
- Swagger, the interactive API page, is at `/umbraco/swagger` in Development. Choose the Umbraco Delivery API document, select Authorize and paste the key.

## Articles API

A small read-only API of our own sits next to the Delivery API. It uses the same `Api-Key` header and the same key. A request without the key, or with a wrong one, gets the status 401. When no key is configured, every request is refused.

| Request | What it returns |
|---|---|
| `GET /api/reading-room/articles` | The published articles of one language, newest first, with the title, intro, publish date, reading minutes, reading level, editor rating and address. `total` counts all of them |
| `GET /api/reading-room/articles/{key}` | One article, also with the body, the key takeaways and the hero image address. The status is 404 when it is not published in that language |

- `culture` chooses the language, for example `es`. Without it the default language is used. An unknown code gets the status 400.
- The list takes `skip` (zero or more) and `take` (1 to 50, 10 when left out). Other values get the status 400.
- In Swagger, choose the Reading Room Articles API document.

The Delivery API returns every property of the content in Umbraco's own shape. This API returns a small shape that this project controls, with the reading time already worked out.

## Where to find each part of the test

| Test item | Where it is |
|---|---|
| Article and Article List document types, templates and views | The document types in the backoffice, `Views/Article.cshtml`, `Views/ArticleList.cshtml`, and the models in the Data project |
| Reading time service and dependency injection | `ReadingTimeService` in Core, registered in `Composers/ReadingTimeComposer.cs`, used in `Views/Article.cshtml` |
| Newsletter form (optional) | `Controllers/NewsletterSurfaceController.cs` and `Views/Partials/NewsletterForm.cshtml` |
| Backoffice extension, Property Editor UI | `App_Plugins/ReadingRoom.Editors/key-takeaways.js` |
| Backoffice extension, Dashboard and Management API | `App_Plugins/ReadingRoom.Editors/article-stats-dashboard.js` and `Controllers/Api/ArticleStatsApiController.cs` |
| Stretch goals | The editor reads placeholder, minimum and maximum. `umbraco-package.json` holds the English and Spanish text. The site has two languages. The tests are in `ReadingRoom.ArticleCMS.Tests` |

Both backoffice extension options were built. The paths in this table are inside `ReadingRoom.ArticleCMS.Web`, except where Core, Data or Tests is named.

## Try the features

- `/` and `/en/` show the English Article List. `/es/` shows the Spanish one. The language switcher in the header links an article to its translation.
- `/en/lorem-ipsum-quick-read/` shows about 1 minute and three key takeaways. The medium read shows about 3 minutes and the long read about 4. The Spanish articles are short, so each shows about 1 minute.
- In the newsletter form, send an empty box or a text that is not an email address to see the validation message, and a valid address to see the thank-you message. The messages follow the language of the page.
- In the backoffice, open the Content section and the "Article stats" tab to see the published articles and the newest titles for English and Spanish.
- In the backoffice, choose Lorem Ipsum Quick Read and use the Takeaways List field. Use the language selector in the name box to edit the Spanish version. Save and publish to see the change on the page.
- In the backoffice, choose the Settings node, type 100 in Words Per Minute and Save and publish. The reading times become about 2, 5 and 8 minutes. Clear the box to go back to the default.
- The default reading speed is `ReadingTime:WordsPerMinute` in `appsettings.json` (200 words per minute).

## Project layout

| Project | Purpose |
|---|---|
| ReadingRoom.ArticleCMS.Web | The Umbraco site: startup, views, controllers, the composers, the adapters to Umbraco, the backoffice extension in `App_Plugins` and the uSync files |
| ReadingRoom.ArticleCMS.Core | Plain C# with no Umbraco dependency: the reading time service, its options, the contract for the reading speed, the API key check, the takeaway lines, the reading levels, the star rating and shared constants |
| ReadingRoom.ArticleCMS.Data | Umbraco-facing models: the Models Builder output for the document types, the view model of the newsletter form and the response models of the APIs |
| ReadingRoom.ArticleCMS.Tests | xUnit tests for Core. The test folders mirror Core, and test doubles are in `Fakes` |
| ReadingRoom.ArticleCMS.IntegrationTests | xUnit tests that run the whole site: pages, APIs, access rules and the adapter that reads the reading speed |

Web references Core and Data. Data references Core. Tests references Core. IntegrationTests references Web. Core references nothing. Umbraco itself is the data store, so there is no separate repository layer.

## Decisions

- Models Builder runs in Development only (`appsettings.Development.json`) and writes its models into the Data project. Production keeps `ModelsMode` set to `Nothing`, so models are compiled and deployed and not generated live.
- The reading time service lives in Core and does not know about Umbraco, so it can be unit tested on its own. It receives the body text as a string. To get the speed that an editor has set, it asks a small contract in Core, `IReadingSpeedSource`. The Web project supplies the implementation, `Adapters/SiteSettingsReadingSpeedSource`, which reads the published Site Settings document. When no value is set, or it is not above zero, the service uses `ReadingTime:WordsPerMinute` from `appsettings.json`.
- `Composers/ReadingTimeComposer.cs` is the one place that registers the service and the adapter. Umbraco runs composers at startup, so this is the composition root. Both are scoped, not singletons, because the adapter reads published content that belongs to the current request. The view receives the service through `@inject`.
- Vary by culture: Spanish is the second language, with no fallback language. Title, intro, takeaways and body vary, and the publish date, the hero image, the reading level and the editor rating are shared. The Articles root has two hostnames, `/en` and `/es`.
- Labels in the views come from Umbraco dictionary items, read with `GetDictionaryValueOrDefault` and the English text as the default. A missing or empty item still shows English. The newsletter validation messages and the thank-you message travel as dictionary keys and are translated when the form is rendered, so they follow the language of the page.
- The stats endpoint sets the variation context for each language, so the titles come from that language. An article that is not published in a language is not counted for it. The endpoint needs a signed-in user who can open the Content section.
- `Composers`, `Adapters` and `Controllers` hold the code that talks to Umbraco. An adapter is a class that implements a Core contract with Umbraco types. If there were more of them, I would move them into their own project.
- The key takeaways editor is plain JavaScript with no build step. It reuses Umbraco's Multiple Text String storage (`Umbraco.MultipleTextstring`), so Umbraco handles saving and gives the model a ready `IEnumerable<string>`.
- Package versions live in one place, `Directory.Packages.props`.
- Warnings are errors in Core, Data and Tests. The Web project is exempt because it contains Umbraco template code that is not ours. The rules in `.editorconfig` are checked by the build for those three projects and shown as warnings in the editor for Web.
- Document types, data types, languages, hostnames, dictionary items and content are exported with uSync into `ReadingRoom.ArticleCMS.Web/uSync/v17`, so structure changes show up as readable text in a diff. uSync exports when an item is saved in the backoffice. It does not import when the site starts here, so the committed database is what a clone runs.
- The SQLite database and the three hero images in `wwwroot/media` are committed, so a clone runs with content straight away. The database is cleaned before each commit: the rows that hold the machine name are emptied and the file is rewritten, so no deleted data stays inside it.
- `Umbraco:CMS:Imaging:HMACSecretKey` in `appsettings.json` signs image resize addresses. It is not a login. A production deployment must supply its own value, for example through the environment variable `Umbraco__CMS__Imaging__HMACSecretKey`.

## Assumptions

- An average reader reads 200 words per minute. This is a common rule of thumb, not a standard, so it is configurable.
- Reading time counts the body text only. The title and intro are not counted.
- The body is rich text, so the service removes the Hypertext Markup Language (HTML) tags and decodes entities such as `&amp;` before it counts words.
- Reading time is rounded up, so any article with words shows at least 1 minute. An empty body shows no reading time.
- Key takeaways keep the order the editor gave them. Empty lines are not shown on the page.
- A Site Settings value that is empty, missing or not above zero means "use the default".
- The site has two languages, English and Spanish. The Spanish text is a first-pass translation and has not been reviewed by a native speaker.
- The newsletter form only records that a sign-up arrived. It does not store the address or send an email.
- The hero images are simple placeholder images (`hero-blue.png`, `hero-green.png`, `hero-orange.png`).
- Local development on Windows with SQLite. No other database was tried.

## Trade-offs

- The reading speed now has two places: the Site Settings document and `appsettings.json`. The editor value wins when it is set. I chose this so editors can change the speed without a deployment while the service stays free of Umbraco code. The cost is one more thing to check when the speed looks wrong.
- The takeaways minimum and maximum are checked twice. The editor shows the reason and stops publishing, and Umbraco checks the same limits on the server. An article outside the limits can still be saved as a draft.
- The editor is plain JavaScript with no build step. That means no extra tooling and very little code, but it has no type checking.
- Reusing Umbraco's Multiple Text String storage means less code. The cost is that the lines are saved as one text separated by line breaks, so a takeaway cannot contain a line break.
- Committing the database and media makes the project easy to run. The cost is binary files in Git and a cleaning step before each commit.
- uSync does not import at startup, so a change that exists only in the uSync files does not reach a clone. The database has to be refreshed with it.
- Models Builder output is committed in the Data project, so the build does not depend on a running site. The cost is generated files in the history.
- The English default text is written in the views as well as in the dictionary. This keeps the site readable when a dictionary item is missing, but the text exists in two places.

## Improvements

- Have the Spanish text reviewed by a native speaker.
- Store the newsletter sign-ups, or send them to an email service.
- Test the numbers that the stats endpoint returns. The tests only check that it refuses callers who are not signed in to the backoffice.
- Build the database from the uSync files, with the steps written down and tested, so a refresh does not depend on a hand-cleaned binary file.
