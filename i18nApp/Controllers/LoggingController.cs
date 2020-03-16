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
    abstract public class LoggingController : Controller
    {
    }
}