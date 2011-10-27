using System.Web.Security;
using Contrive.Core;

namespace Contrive.Web
{
  public class AuthenticationService:IAuthenticationService
  {
    readonly IUserService _userService;

    public AuthenticationService(IUserService userService)
    {
      _userService = userService;
    }

    public bool LogOn(string userName, string password, bool rememberMe = false)
    {
      if (_userService.ValidateUser(userName, password))
      {
        FormsAuthentication.SetAuthCookie(userName, rememberMe);
        return true;
      }
      return false;
    }

    public void LogOff()
    {
      FormsAuthentication.SignOut();
    }
  }
}