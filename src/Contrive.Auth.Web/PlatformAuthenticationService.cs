using System.Web.Security;

namespace Contrive.Auth.Web
{
  public class PlatformAuthenticationService : IPlatformAuthenticationService
  {
    public bool SignIn(string userName, string password, bool rememberMe = false)
    {
      FormsAuthentication.SetAuthCookie(userName, rememberMe);
      return true;
    }

    public void SignOut()
    {
      FormsAuthentication.SignOut();
    }
  }
}