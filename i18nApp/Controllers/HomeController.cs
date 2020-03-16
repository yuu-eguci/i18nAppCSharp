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
    public class HomeController : LoggingController
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