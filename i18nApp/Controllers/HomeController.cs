using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;
using i18nApp.Helpers;

namespace i18nApp.Controllers
{
    /// <summary>
    /// Run log4net in the Order:1 OnActionExecuting in all derived classes.
    /// Set Order:1 to run this after I18nAttribute.
    /// Only if using I18nAttribute for all action methods, just writing like below will work.
    ///     [I18nAttribute]
    ///     [AccessLoggingAttribute]
    /// </summary>
    [AccessLoggingAttribute(Order = 1)]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// This page switches its Culture depending on the culture in the url.
        /// NOTE: language isn't used in the method though,
        /// it cannot be used in I18nAttribute if the argument isn't set.
        /// HACK: Does it exist any smart solution?
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        [I18nAttribute]
        public ActionResult About(string culture)
        {
            return View();
        }

        /// <summary>
        /// Same as /Home/About above.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        [I18nAttribute]
        public ActionResult Contact(string culture)
        {
            return View();
        }
    }
}