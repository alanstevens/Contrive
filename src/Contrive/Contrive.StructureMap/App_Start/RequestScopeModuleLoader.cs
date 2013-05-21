using Contrive.StructureMap;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof (RequestScopeModuleLoader), "PreStart")]

namespace Contrive.StructureMap
{
    public static class RequestScopeModuleLoader
    {
        public static void PreStart()
        {
            DynamicModuleUtility.RegisterModule(typeof (RequestScopeModule));
        }
    }
}