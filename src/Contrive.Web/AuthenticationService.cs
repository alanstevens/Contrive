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

    public void LogOn(string userName, string password, bool rememberMe = false)
    {
      if (_userService.ValidateUser(userName, password))
      {
        FormsAuthentication.SetAuthCookie(userName, rememberMe);
      }
    }

    public void SignOut()
    {
      FormsAuthentication.SignOut();
    }
  }
}