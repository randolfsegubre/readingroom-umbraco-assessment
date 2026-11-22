# ReadingRoom.ArticleCMS

An article site built on Umbraco 17 and .NET 10 for the Reading Room Umbraco technical test.

Status: Work in progress. Sections marked "To be written" are filled in as each feature is finished.

## Requirements

- .NET Software Development Kit (SDK) 10.0.403 or a newer 10.0 release (pinned in `global.json`).
- Visual Studio 2026, or any editor plus the `dotnet` command line

## Run it

From the repository root:

    dotnet run --project ReadingRoom.ArticleCMS.Web

Open https://localhost:44308. The backoffice is at https://localhost:44308/umbraco.

In Visual Studio, open `ReadingRoom.ArticleCMS.slnx`, make the Web project the startup project, choose the "IIS Express" profile and press F5 to run. It uses the same address.

Use only one of the two at a time. They share one port and one database.

## First run

In Development, the first run creates the SQLite database and the administrator account by itself (`Umbraco:CMS:Unattended` in `appsettings.Development.json`). Sign in at `/umbraco` with:

- Email: randolf.segubre@example.test
- Password: Welcome123!

This is a throwaway account for local development only. Do not reuse it anywhere else.

## Project layout

| Project | Purpose |
|---|---|
| ReadingRoom.ArticleCMS.Web | The Umbraco site: startup, views and backoffice extensions |
| ReadingRoom.ArticleCMS.Core | Plain C# with no Umbraco dependency (will hold the reading time service) |
| ReadingRoom.ArticleCMS.Data | Models Builder output for the Umbraco document types (generated models will go here) |
| ReadingRoom.ArticleCMS.Tests | xUnit tests for Core |

Web references Core and Data. Tests references Core. Core references nothing. Umbraco itself is the data store, so there is no separate repository layer.

## Decisions so far

- Models Builder runs in Development only (`appsettings.Development.json`) and writes its models into the Data project. Production keeps `ModelsMode` set to `Nothing`, so models are compiled and deployed and not generated live.
- Warnings are errors in Core, Data and Tests. The Web project is exempt because it contains Umbraco template code that is not ours. The rules in `.editorconfig` are checked by the build for those three projects and shown as warnings in the editor for Web.
- Package versions live in one place, `Directory.Packages.props`.
- `Umbraco:CMS:Imaging:HMACSecretKey` in `appsettings.json` signs image resize URLs. It is not a login. A production deployment must supply its own value, for example through the environment variable `Umbraco__CMS__Imaging__HMACSecretKey`.

## Assumptions

To be written.

## Trade-offs

To be written.

## Improvements

To be written.
