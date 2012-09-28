using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Contrive.Common;
using Contrive.StructureMap;

namespace Contrive.Auth.Web.Mvc
{
  public class MvcApplication : HttpApplication
  {
    static MvcApplication()
    {
      Bootstrapper.Run(ContriveConfigurator.Configure, "Contrive");
    }

    protected void Application_Start()
    {
      RegisterGlobalFilters(GlobalFilters.Filters);
      RegisterRoutes(RouteTable.Routes);
    }

    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute());
    }

    public static void RegisterRoutes(RouteCollection routes)
    {
      AreaRegistration.RegisterAllAreas();

      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      routes.MapRoute("Default", // Route name
                      "{controller}/{action}/{id}", // URL with parameters
                      new {controller = "Home", action = "Index", id = UrlParameter.Optional} // Parameter defaults
        );
    }
  }
}