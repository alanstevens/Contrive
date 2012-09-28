using System.Web.Mvc;
using System.Web.Routing;
using Contrive.Common;

namespace Contrive.Web.Common.Mvc
{
  public class WebStartupTask : IStartupTask
  {
    public void OnStartup()
    {
      RouteCollection routes = RouteTable.Routes;

      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
      routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

      routes.MapRoute(
        "Default", // Route name
        "{controller}/{action}/{id}", // URL with parameters
        new
        {
          controller = "Home",
          action = "Index",
          id = UrlParameter.Optional
        });
    }
  }
}