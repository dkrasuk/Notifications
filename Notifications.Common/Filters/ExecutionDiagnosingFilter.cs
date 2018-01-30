using System.Configuration;
using System.Diagnostics;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using AlfaBank.Logger;
using Microsoft.Practices.ServiceLocation;

namespace Notifications.Common.Filters
{
    public class ExecutionDiagnosingFilter : ActionFilterAttribute
    {
        private Stopwatch _sw;

        private readonly ILogger _logger;

        private static bool DisableExecutionDiagnosingLoggingOption
            => ConfigurationManager.AppSettings["DisableExecutionDiagnosingLogging"] == "true";

        public ExecutionDiagnosingFilter()
        {
            _logger = ServiceLocator.Current.GetInstance<ILogger>();
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (DisableExecutionDiagnosingLoggingOption)
            {
                return;
            }

            _logger.Debug($"Action execution started. Url: {actionContext.RequestContext.Url.Request.RequestUri.AbsolutePath}");

            _sw = Stopwatch.StartNew();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (DisableExecutionDiagnosingLoggingOption)
            {
                return;
            }

            _logger.Debug($"Action execution stopped. Url: {actionExecutedContext.Request.RequestUri.AbsolutePath} Elapsed: {_sw.Elapsed}");
        }
    }
}
