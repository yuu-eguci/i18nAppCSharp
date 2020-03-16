i18nApp C#
===

Internationalization application served by C# ASP.NET MVC on Azure and is deployed through DevOps.

## What this project includes

1. **/Home/Index** that switches its language depending on http request header: Accept-Language
2. **/Home/About** that
    1. switches its language depending on its URL such as /ja/Home/About and /en/Home/About
    2. redirects to /{default culture}/Home/About automatically
3. **/Home/Contact** that
    - is same as About page

![1](https://user-images.githubusercontent.com/28250432/76743568-28890a80-67b6-11ea-9e49-a99e2c17897b.png)

![2](https://user-images.githubusercontent.com/28250432/76743587-33dc3600-67b6-11ea-836d-ef1d24dbfb6d.png)

## Installation

After cloning the repository, restore Nuget packages.

![restore-nuget-packages](https://user-images.githubusercontent.com/28250432/76743594-36d72680-67b6-11ea-881e-4dea97a247fe.png)
