using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;

namespace i18nApp.Helpers
{
    /// <summary>
    /// This attribute changes Thread.CurrentThread.CurrentCulture
    /// depending on the accessed URL on action executing.
    /// </summary>
    public class I18nAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Cultures this app serves. RouteConfig limits {culture} in the url to these.
        /// The first one is the default.
        /// </summary>
        public static readonly string[] SupportedCultures = { "ja", "en" };

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // NOTE:
            // Can get data from filterContext such as...
            // filterContext.ActionParameters["culture"]
            // filterContext.HttpContext.Request.QueryString["key"]

            // Redirect to /{culture}/{controller}/{action} when request is /{controller}/{action}
            if (!(filterContext.ActionParameters["culture"] is string culture))  // Means that if culture is null...
            {
                // Original request uri.
                Uri requestUri = filterContext.HttpContext.Request.Url;

                // Change uri path.
                // Default culture is ja. But if cookie has supported culture information, use it.
                // The cookie is client input, so any other value is ignored.
                string initialDefaultCulture = SupportedCultures[0];
                HttpCookie cookieCulture = filterContext.RequestContext.HttpContext.Request.Cookies["culture"];
                if (cookieCulture != null)
                {
                    initialDefaultCulture = SupportedCultures.FirstOrDefault(
                        c => string.Equals(c, cookieCulture.Value, StringComparison.OrdinalIgnoreCase)) ?? initialDefaultCulture;
                }

                // Redirect to changed uri.
                // Relative path, so that the Host header of the request is not echoed back.
                filterContext.Result = new RedirectResult("/" + initialDefaultCulture + requestUri.PathAndQuery);
                return;
            }

            // Needless to check culture string because it has been filtered to ja or en already in RouteConfig.
            // NOTE: CurrentCulture is used to switch formats.
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            // NOTE: CurrentUICulture is used to switch resources.
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);

            // I like to change language depending on the uri, so cookie is not required
            // But storing culture in the cookie seems to be common. Try to implement.
            // Store the culture in the cookie which is specified in the url.
            // Scripts don't need it, and it is sent only to this site's own navigation.
            var cookie = new HttpCookie("culture")
            {
                Value = culture,
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Secure = filterContext.HttpContext.Request.IsSecureConnection
            };
            filterContext.HttpContext.Response.Cookies.Add(cookie);
        }
    }
}
