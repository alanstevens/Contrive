namespace Contrive.Core
{
  public interface IAuthenticationService
  {
    bool LogOn(string userName, string password, bool rememberMe = false);

    void LogOff();
  }
}