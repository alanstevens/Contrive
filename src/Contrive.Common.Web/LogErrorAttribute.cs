using System.Web.Mvc;
using Contrive.Common.Extensions;

namespace Contrive.Common.Web
{
    public class LogErrorAttribute : HandleErrorAttribute, IStartupTask
    {
        public void OnStartup()
        {
            GlobalFilters.Filters.Add(new LogErrorAttribute());
        }

        public override void OnException(ExceptionContext exceptionContext)
        {
            base.OnException(exceptionContext);

            // We want to log exceptions that were handled by the base implementation.
            // Unhandled exceptions are always logged.
            if (exceptionContext.ExceptionHandled) this.LogException(exceptionContext.Exception);
        }
    }
}