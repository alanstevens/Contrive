namespace Contrive.Core
{
  public interface IPlatformAuthenticationService
  {
    void SignIn(string userName, bool rememberMe = false);

    void SignOut();
  }
}