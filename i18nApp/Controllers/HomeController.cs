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
    public class HomeController : Controller
    {
        /// <summary>
        /// Logger for this class.
        /// </summary>
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(
            // Name this logger with this class name.
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Index()
        {
            logger.Info($"Called method: {System.Reflection.MethodBase.GetCurrentMethod().Name}, Culture: {Thread.CurrentThread.CurrentCulture.Name}");

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
            logger.Info($"Called method: {System.Reflection.MethodBase.GetCurrentMethod().Name}, Culture: {Thread.CurrentThread.CurrentCulture.Name}");

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
            logger.Info($"Called method: {System.Reflection.MethodBase.GetCurrentMethod().Name}, Culture: {Thread.CurrentThread.CurrentCulture.Name}");

            return View();
        }
    }
}