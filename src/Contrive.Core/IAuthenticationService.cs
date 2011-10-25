namespace Contrive.Core
{
  public interface IAuthenticationService
  {
    void LogOn(string userName, string password, bool rememberMe = false);

    void SignOut();
  }
}