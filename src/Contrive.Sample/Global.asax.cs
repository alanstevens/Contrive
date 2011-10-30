using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Contrive.Core;
using Contrive.EntityFramework;
using Contrive.StructureMap;
using StructureMap;

namespace Contrive.Sample
{
  public class MvcApplication : HttpApplication
  {
    protected void Application_Start()
    {
      IContainer container = new Container();

      container.Configure(c =>
      {
        c.For<DbContext>().HybridHttpOrThreadLocalScoped().Use<EntityContext>();
        c.For(typeof(IRepository<>)).HybridHttpOrThreadLocalScoped().Use(typeof(Repository<>));
        c.For<IUserRepository>().HybridHttpOrThreadLocalScoped().Use<UserRepository>();
        c.For<IRoleRepository>().HybridHttpOrThreadLocalScoped().Use<RoleRepository>();
        c.For<IRole>().Use<Role>();
        c.For<IUser>().Use<User>();
      });

      Configurator.ConfigureWith(container);

      AreaRegistration.RegisterAllAreas();

      RegisterGlobalFilters(GlobalFilters.Filters);
      RegisterRoutes(RouteTable.Routes);
    }

    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute());
    }

    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      routes.MapRoute(
        "Default", // Route name
        "{controller}/{action}/{id}", // URL with parameters
        new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
        );
    }
  }
}