using System;
using System.Web;
using Contrive.Common.Extensions;
using Elmah;

namespace Contrive.Common.Web
{
    public class Elmah : IStartupTask
    {
        public static Action<Exception> RaiseErrorSignal = e => { };

        public void OnStartup()
        {
            RaiseErrorSignal = ex =>
                               {
                                   var context = HttpContext.Current;

                                   if (context.IsNull()) return;

                                   var signal = ErrorSignal.FromContext(context);

                                   if (signal.IsNull()) return;

                                   signal.Raise(ex, context);
                               };
        }
    }
}