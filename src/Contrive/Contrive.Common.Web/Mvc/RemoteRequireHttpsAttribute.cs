using System.Web.Mvc;
using Contrive.Common.Extensions;

namespace Contrive.Common.Web.Mvc
{
    public class RemoteRequireHttpsAttribute : RequireHttpsAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            Verify.NotNull(filterContext, "filterContext");

            var context = filterContext.HttpContext;
            if (context.IsNotNull() && context.Request.IsLocal) return;

            base.OnAuthorization(filterContext);
        }
    }
}