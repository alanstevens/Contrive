using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using Contrive.Web.Common;

namespace Contrive.Auth.Web
{
  public class PlatformAuthenticationService : IPlatformAuthenticationService
  {
    static HttpContextBase HttpContext { get { return new HttpContextWrapper(System.Web.HttpContext.Current); } }

    public IPrincipal CurrentPrincipal { get { return HttpContext.User; } }

    public bool UserIsAuthenticated { get { return HttpContext.Request.IsAuthenticated; } }

    public bool SignIn(string userName, string password, bool rememberMe = false)
    {
      FormsAuthentication.SetAuthCookie(userName, rememberMe);
      return true;
    }

    public void SignOut()
    {
      FormsAuthentication.SignOut();
    }

    public void Unauthorize()
    {
      HttpContext.Response.SetStatus(HttpStatusCode.Unauthorized);
    }
  }
}