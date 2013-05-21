using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Contrive.Auth.Web;
using Contrive.Common;
using Contrive.Common.Web;
using Contrive.Sample.Web.App_Start;
using Contrive.StructureMap;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Sample.Web
{
    public class ContriveSampleApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            Bootstrapper.Configurator = StructureMapContainerConfigurator.ConfigureWith;
            Bootstrapper.Run("Contrive");

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs args)
        {
            if (!User.Identity.IsAuthenticated) return;

            var formsAuthService = ServiceLocator.Current.GetInstance<IFormsAuthService>();

            Context.User = formsAuthService.GetUpdatedPrincipalFor(User);
        }
    }
}