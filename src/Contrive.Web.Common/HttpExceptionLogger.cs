using System;
using System.Diagnostics;
using System.Web;
using Contrive.Common;
using Contrive.Common.Extensions;
using Elmah;

namespace Contrive.Web.Common
{
    public class HttpExceptionLogger : IStartupTask
    {
        public void OnStartup()
        {
            LoggingExtensions.ExceptionLogger = LogException;
        }

        static void LogException(object source, Exception ex)
        {
            try
            {
                RaiseErrorSignal(ex);

                // Rely on ELMAH configuration to filter messages from Log4Net
                if (IsFiltered(ex)) return;

                source = Type.GetType(ex.GetBaseException().Source, false) ?? Type.GetType(ex.Source, false) ?? source;
                Logger.Error(source, ex.Serialize());
            }
            catch
            {
#if DEBUG
                Debugger.Break();
#endif
            }
        }

        static void RaiseErrorSignal(Exception ex)
        {
            var context = HttpContext.Current;

            if (context.IsNull()) return;

            var signal = ErrorSignal.FromContext(context);

            if (signal.IsNull()) return;

            signal.Raise(ex, context);
        }

        static bool IsFiltered(Exception ex)
        {
            var context = HttpContext.Current;

            if (context.IsNull()) return false;

            var config = context.GetSection("elmah/errorFilter") as ErrorFilterConfiguration;

            if (config.IsNull()) return false;

            var testContext = new ErrorFilterModule.AssertionHelperContext(ex, context);

            return config.Assertion.Test(testContext);
        }
    }
}