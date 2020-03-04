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
                // Default culture is ja.
                uriBuilder.Path = "ja" + uriBuilder.Path;

                // Redirect to changed uri.
                filterContext.Result = new RedirectResult(uriBuilder.ToString());
                return;
            }

            // Needless to check culture string because it has been filtered to ja or en already in RouteConfig.
            // NOTE: CurrentCulture is used to switch formats.
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            // NOTE: CurrentUICulture is used to switch resources.
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
        }
    }
}