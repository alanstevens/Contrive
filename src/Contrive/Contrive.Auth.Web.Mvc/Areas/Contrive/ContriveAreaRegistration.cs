using System.Web.Mvc;

namespace Contrive.Auth.Web.Mvc.Areas.Contrive
{
    public class ContriveAreaRegistration : AreaRegistration
    {
        public override string AreaName { get { return "Contrive"; } }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute("Contrive_Account", "Account/index/{searchterm}",
                             new {controller = "ContriveAccount", action = "Index", searchterm = UrlParameter.Optional});

            context.MapRoute("Search_Membership", "Contrive/Membership/index/{searchterm}/{filterby}",
                             new
                             {
                                 controller = "Membership",
                                 action = "Index",
                                 searchterm = UrlParameter.Optional,
                                 filterby = UrlParameter.Optional
                             });

            context.MapRoute("Contrive_Membership", "Contrive/Membership/{action}/{userName}",
                             new {controller = "Membership", userName = UrlParameter.Optional});

            context.MapRoute("Contrive_Default", "Contrive/{controller}/{action}/{id}",
                             new {controller = "Dashboard", action = "Index", id = UrlParameter.Optional});
        }
    }
}