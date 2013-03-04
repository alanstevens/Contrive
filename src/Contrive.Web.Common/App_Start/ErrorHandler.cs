using System.Web;
using Contrive.Web.Common.App_Start;
using Contrive.Web.Common.Modules;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

[assembly: PreApplicationStartMethod(typeof (ErrorHandler), "PreStart")]

namespace Contrive.Web.Common.App_Start
{
    public static class ErrorHandler
    {
        public static void PreStart()
        {
            DynamicModuleUtility.RegisterModule(typeof (ErrorHandlingModule));
        }
    }
}