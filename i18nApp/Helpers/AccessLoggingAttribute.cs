using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;

namespace i18nApp.Helpers
{
    /// <summary>
    /// This attribute writes a log4net access log on action executing.
    /// </summary>
    public class AccessLoggingAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controllerType = filterContext.ActionDescriptor.ControllerDescriptor.ControllerType.FullName;
            string actionMethodName = filterContext.ActionDescriptor.ActionName;

            log4net.ILog logger = log4net.LogManager.GetLogger(
                // Name this logger with this class name.
                controllerType);
            logger.Info($"Access: {controllerType}.{actionMethodName}, Culture: {Thread.CurrentThread.CurrentCulture.Name}");
        }
    }
}