using System;

namespace Contrive.Auth
{
  public class AuthenticationService : IAuthenticationService
  {
    public AuthenticationService(IUserService userService, IPlatformAuthenticationService platformAuthenticationService)
    {
      _userService = userService;
      _platformAuthenticationService = platformAuthenticationService;
    }

    readonly IPlatformAuthenticationService _platformAuthenticationService;
    readonly IUserService _userService;

    public bool LogOn(string userName, string password, bool rememberMe = false)
    {
      if (_userService.ValidateUser(userName, password))
        return _platformAuthenticationService.SignIn(userName, password, rememberMe);
      return false;
    }

    public void LogOff()
    {
      _platformAuthenticationService.SignOutCurrentUser();
    }

    public void SignOut(string userName)
    {
      _platformAuthenticationService.SignOut(userName);
    }

    public void SignOut(Guid userId)
    {
      _platformAuthenticationService.SignOut(userId);
    }
  }
}