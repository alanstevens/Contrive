using Contrive.Common.Web.App_Start;
using Contrive.Common.Web.Modules;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof (ErrorHandlingModuleLoader), "PreStart")]

namespace Contrive.Common.Web.App_Start
{
    public static class ErrorHandlingModuleLoader
    {
        public static void PreStart()
        {
            DynamicModuleUtility.RegisterModule(typeof (ErrorHandlingModule));
        }
    }
}