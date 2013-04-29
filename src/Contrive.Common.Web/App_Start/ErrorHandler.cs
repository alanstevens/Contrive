using System.Web;
using Contrive.Common.Web.App_Start;
using Contrive.Common.Web.Modules;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

[assembly: PreApplicationStartMethod(typeof (ErrorHandler), "PreStart")]

namespace Contrive.Common.Web.App_Start
{
    public static class ErrorHandler
    {
        public static void PreStart()
        {
            DynamicModuleUtility.RegisterModule(typeof (ErrorHandlingModule));
        }
    }
}