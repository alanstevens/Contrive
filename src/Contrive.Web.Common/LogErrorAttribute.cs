using System.Web.Mvc;
using Contrive.Common;
using Contrive.Common.Extensions;

namespace Contrive.Web.Common
{
  public class LogErrorAttribute : HandleErrorAttribute, IStartupTask
  {
    public override void OnException(ExceptionContext exceptionContext)
    {
      base.OnException(exceptionContext);

      // We want to log exceptions that were handled by the base implementation.
      // Unhandled exceptions are always logged.
      if (exceptionContext.ExceptionHandled)
        this.LogException(exceptionContext.Exception);
    }

    public void OnStartup()
    {
      GlobalFilters.Filters.Add(new LogErrorAttribute());
    }
  }
}