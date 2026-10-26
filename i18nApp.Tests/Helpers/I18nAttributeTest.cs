using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using i18nApp.Controllers;
using i18nApp.Helpers;

namespace i18nApp.Tests.Helpers
{
    [TestClass]
    public class I18nAttributeTest
    {
        private CultureInfo originalCulture;
        private CultureInfo originalUICulture;

        [TestInitialize]
        public void SaveCultures()
        {
            originalCulture = Thread.CurrentThread.CurrentCulture;
            originalUICulture = Thread.CurrentThread.CurrentUICulture;
        }

        [TestCleanup]
        public void RestoreCultures()
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
            Thread.CurrentThread.CurrentUICulture = originalUICulture;
        }

        [TestMethod]
        public void RedirectsToJapaneseByDefault()
        {
            var context = CreateContext("http://localhost/Home/About?page=1", culture: null);

            new I18nAttribute().OnActionExecuting(context);

            Assert.AreEqual("/ja/Home/About?page=1", ((RedirectResult)context.Result).Url);
        }

        [TestMethod]
        public void RedirectsToCultureInCookie()
        {
            var context = CreateContext("http://localhost/Home/About", culture: null, cookieCulture: "en");

            new I18nAttribute().OnActionExecuting(context);

            Assert.AreEqual("/en/Home/About", ((RedirectResult)context.Result).Url);
        }

        [TestMethod]
        public void NormalizesCultureInCookie()
        {
            // Route constraints ignore case, so /EN/Home/About can store "EN" in the cookie.
            var context = CreateContext("http://localhost/Home/About", culture: null, cookieCulture: "EN");

            new I18nAttribute().OnActionExecuting(context);

            Assert.AreEqual("/en/Home/About", ((RedirectResult)context.Result).Url);
        }

        [TestMethod]
        public void IgnoresUnsupportedCultureInCookie()
        {
            var context = CreateContext("http://localhost/Home/About", culture: null, cookieCulture: "evil.example");

            new I18nAttribute().OnActionExecuting(context);

            Assert.AreEqual("/ja/Home/About", ((RedirectResult)context.Result).Url);
        }

        [TestMethod]
        public void DoesNotEchoHostHeaderInRedirect()
        {
            var context = CreateContext("http://attacker.example/Home/About", culture: null);

            new I18nAttribute().OnActionExecuting(context);

            Assert.AreEqual("/ja/Home/About", ((RedirectResult)context.Result).Url);
        }

        [TestMethod]
        public void SetsThreadCulturesFromUrl()
        {
            var context = CreateContext("http://localhost/en/Home/About", culture: "en");

            new I18nAttribute().OnActionExecuting(context);

            Assert.IsNull(context.Result);
            Assert.AreEqual("en", Thread.CurrentThread.CurrentCulture.Name);
            Assert.AreEqual("en", Thread.CurrentThread.CurrentUICulture.Name);
        }

        [TestMethod]
        public void StoresCultureInHardenedCookie()
        {
            var context = CreateContext("http://localhost/en/Home/About", culture: "en");

            new I18nAttribute().OnActionExecuting(context);

            HttpCookie cookie = context.HttpContext.Response.Cookies["culture"];
            Assert.AreEqual("en", cookie.Value);
            Assert.IsTrue(cookie.HttpOnly);
            Assert.AreEqual(SameSiteMode.Lax, cookie.SameSite);
            Assert.IsFalse(cookie.Secure);
        }

        [TestMethod]
        public void StoresSecureCookieOverHttps()
        {
            var context = CreateContext("https://localhost/en/Home/About", culture: "en");

            new I18nAttribute().OnActionExecuting(context);

            Assert.IsTrue(context.HttpContext.Response.Cookies["culture"].Secure);
        }

        private static ActionExecutingContext CreateContext(string url, string culture, string cookieCulture = null)
        {
            var request = new FakeRequest(new Uri(url));
            if (cookieCulture != null)
            {
                request.Cookies.Add(new HttpCookie("culture", cookieCulture));
            }
            var controllerContext = new ControllerContext(new FakeHttpContext(request), new RouteData(), new HomeController());
            var actionDescriptor = new ReflectedActionDescriptor(
                typeof(HomeController).GetMethod(nameof(HomeController.About)),
                nameof(HomeController.About),
                new ReflectedControllerDescriptor(typeof(HomeController)));
            var parameters = new Dictionary<string, object> { { "culture", culture } };
            return new ActionExecutingContext(controllerContext, actionDescriptor, parameters);
        }

        private class FakeHttpContext : HttpContextBase
        {
            private readonly HttpRequestBase request;
            private readonly HttpResponseBase response = new FakeResponse();

            public FakeHttpContext(HttpRequestBase request)
            {
                this.request = request;
            }

            public override HttpRequestBase Request => request;
            public override HttpResponseBase Response => response;
        }

        private class FakeRequest : HttpRequestBase
        {
            private readonly Uri url;
            private readonly HttpCookieCollection cookies = new HttpCookieCollection();

            public FakeRequest(Uri url)
            {
                this.url = url;
            }

            public override Uri Url => url;
            public override bool IsSecureConnection => url.Scheme == Uri.UriSchemeHttps;
            public override HttpCookieCollection Cookies => cookies;
        }

        private class FakeResponse : HttpResponseBase
        {
            private readonly HttpCookieCollection cookies = new HttpCookieCollection();

            public override HttpCookieCollection Cookies => cookies;
        }
    }
}
