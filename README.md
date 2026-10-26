i18nApp C#
===

[![CI](https://github.com/yuu-eguci/i18nAppCSharp/actions/workflows/ci.yml/badge.svg)](https://github.com/yuu-eguci/i18nAppCSharp/actions/workflows/ci.yml)

Internationalization application served by C# ASP.NET MVC (.NET Framework).

It was originally served on Azure and deployed through Azure DevOps. That pipeline has been retired; CI now only builds and tests on GitHub Actions and never deploys.

## What this project includes

1. **/Home/Index** that switches its language depending on http request header: Accept-Language
2. **/Home/About** that
    1. switches its language depending on its URL such as /ja/Home/About and /en/Home/About
    2. redirects to /{default culture}/Home/About automatically
3. **/Home/Contact** that
    - is same as About page

![1](https://user-images.githubusercontent.com/28250432/76743568-28890a80-67b6-11ea-9e49-a99e2c17897b.png)

![2](https://user-images.githubusercontent.com/28250432/76743587-33dc3600-67b6-11ea-836d-ef1d24dbfb6d.png)

## Requirements

- Windows
- Visual Studio 2022 (or Build Tools) with the ASP.NET and web development workload
- .NET Framework 4.7.2 targeting pack

This is a .NET Framework / System.Web application, so it does not build or run on macOS or Linux.

## Installation

After cloning the repository, restore Nuget packages.

![restore-nuget-packages](https://user-images.githubusercontent.com/28250432/76743594-36d72680-67b6-11ea-881e-4dea97a247fe.png)

Then run the `i18nApp` project with IIS Express from Visual Studio.

## Build and test from the command line

Run these in a Developer PowerShell for Visual Studio:

```powershell
nuget restore i18nApp.sln
msbuild i18nApp.sln /p:Configuration=Release /p:DeployOnBuild=true /p:WebPublishMethod=FileSystem /p:PublishUrl="$PWD\publish" /p:PrecompileBeforePublish=true
vstest.console i18nApp.Tests\bin\Release\i18nApp.Tests.dll
```

`PrecompileBeforePublish` compiles the Razor views as well, so view errors fail the build. The published site is written to the local `publish` folder only.

## CI

[`.github/workflows/ci.yml`](.github/workflows/ci.yml) runs the commands above on a GitHub-hosted Windows runner for pushes to `master` and for pull requests. It uses no secrets and has no deploy step.
