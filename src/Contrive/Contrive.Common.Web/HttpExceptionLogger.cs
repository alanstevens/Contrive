using System;
using System.Diagnostics;
using Contrive.Common.Extensions;
using Elmah;

namespace Contrive.Common.Web
{
    public class HttpExceptionLogger : IStartupTask
    {
        public void OnStartup()
        {
            LoggingExtensions.ExceptionLogger = LogException;
        }

        void LogException(object source, Exception ex)
        {
            try
            {
                Elmah.RaiseErrorSignal(ex);

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

        bool IsFiltered(Exception ex)
        {
            var context = HttpContextProvider.GetContext();

            if (context.IsNull()) return false;

            var config = context.GetSection("elmah/errorFilter") as ErrorFilterConfiguration;

            if (config.IsNull()) return false;

            var testContext = new ErrorFilterModule.AssertionHelperContext(ex, context);

            return config.Assertion.Test(testContext);
        }
    }
}