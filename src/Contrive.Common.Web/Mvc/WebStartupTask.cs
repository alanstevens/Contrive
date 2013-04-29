using System.Web.Mvc;
using System.Web.Routing;

namespace Contrive.Common.Web.Mvc
{
    public class WebStartupTask : IStartupTask
    {
        public void OnStartup()
        {
            var routes = RouteTable.Routes;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new {favicon = @"(.*/)?favicon.ico(/.*)?"});

            routes.MapRoute("Default", // Route name
                            "{controller}/{action}/{id}", // URL with parameters
                            new {controller = "Home", action = "Index", id = UrlParameter.Optional});
        }
    }
}