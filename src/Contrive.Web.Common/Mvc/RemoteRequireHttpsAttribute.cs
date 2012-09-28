using System.Web.Mvc;
using Contrive.Common;
using Contrive.Common.Extensions;

namespace Contrive.Web.Common.Mvc
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