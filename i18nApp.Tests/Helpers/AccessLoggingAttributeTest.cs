using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using i18nApp.Controllers;
using i18nApp.Helpers;

namespace i18nApp.Tests.Helpers
{
    [TestClass]
    public class AccessLoggingAttributeTest
    {
        private ILoggerRepository repository;
        private MemoryAppender appender;
        private CultureInfo originalCulture;

        [TestInitialize]
        public void ConfigureMemoryAppender()
        {
            repository = LogManager.GetRepository(typeof(AccessLoggingAttribute).Assembly);
            appender = new MemoryAppender();
            BasicConfigurator.Configure(repository, appender);
            originalCulture = Thread.CurrentThread.CurrentCulture;
        }

        [TestCleanup]
        public void ResetLogging()
        {
            repository.ResetConfiguration();
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }

        [TestMethod]
        public void WritesAccessLogWithCulture()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en");
            var actionDescriptor = new ReflectedActionDescriptor(
                typeof(HomeController).GetMethod(nameof(HomeController.About)),
                nameof(HomeController.About),
                new ReflectedControllerDescriptor(typeof(HomeController)));
            var context = new ActionExecutingContext(new ControllerContext(), actionDescriptor, new Dictionary<string, object>());

            new AccessLoggingAttribute().OnActionExecuting(context);

            LoggingEvent loggingEvent = appender.GetEvents().Single();
            Assert.AreEqual(Level.Info, loggingEvent.Level);
            Assert.AreEqual("i18nApp.Controllers.HomeController", loggingEvent.LoggerName);
            Assert.AreEqual("Access: i18nApp.Controllers.HomeController.About, Culture: en", loggingEvent.RenderedMessage);
        }
    }
}
