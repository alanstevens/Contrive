using Contrive.Web.Common.Modules;
using Contrive.Web.Common.App_Start;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using WebActivator;

[assembly: PreApplicationStartMethod(typeof (ErrorHandler), "PreStart")]

namespace GoldMail.CloudComposer.WebRole.App_Start
{
    public static class ErrorHandler
    {
        public static void PreStart()
        {
            DynamicModuleUtility.RegisterModule(typeof (ErrorHandlingModule));
        }
    }
}