using System.Web.Mvc;

namespace Contrive.Sample.Areas.Contrive
{
  public class ContriveAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "Contrive";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {

      context.MapRoute("SearchMembership", "Contrive/Membership/index/{searchterm}/{filterby}",
          new { controller = "Membership", action = "Index", searchterm = UrlParameter.Optional, filterby = UrlParameter.Optional }
          );

      context.MapRoute("Membership", "Contrive/Membership/{action}/{userName}",
          new { controller = "Membership", userName = UrlParameter.Optional }
          );

      context.MapRoute(
          "Contrive_default",
          "Contrive/{controller}/{action}/{id}",
          new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}
