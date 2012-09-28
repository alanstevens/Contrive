using System.Web.Security;
using Contrive.Core;

namespace Contrive.Web
{
  public class PlatformAuthenticationService : IPlatformAuthenticationService
  {
    public void SignIn(string userName, bool rememberMe = false)
    {
      FormsAuthentication.SetAuthCookie(userName, rememberMe);
    }

    public void SignOut()
    {
      FormsAuthentication.SignOut();
    }
  }
}