namespace Contrive.Core
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
      {
        _platformAuthenticationService.SignIn(userName, rememberMe);
        return true;
      }
      return false;
    }

    public void LogOff()
    {
      _platformAuthenticationService.SignOut();
    }
  }
}