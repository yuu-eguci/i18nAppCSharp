using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;

namespace i18nApp.Helpers
{
    public class I18nAttribute : ActionFilterAttribute
    {
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
                var uriBuilder = new UriBuilder(requestUri);

                // Change uri path.
                // Default culture is ja. But if cookie has culture information, use it.
                string initialDefaultCulture = "ja";
                HttpCookie cookieCulture = filterContext.RequestContext.HttpContext.Request.Cookies["culture"];
                if (cookieCulture != null && !string.IsNullOrEmpty(cookieCulture.Value))
                {
                    initialDefaultCulture = cookieCulture.Value;
                }
                uriBuilder.Path = initialDefaultCulture + uriBuilder.Path;

                // Redirect to changed uri.
                filterContext.Result = new RedirectResult(uriBuilder.ToString());
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
            var cookie = new HttpCookie("culture")
            {
                Value = culture
            };
            filterContext.HttpContext.Response.Cookies.Add(cookie);
        }
    }
}