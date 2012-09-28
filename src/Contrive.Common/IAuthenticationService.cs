namespace Contrive.Common
{
  public interface IAuthenticationService
  {
    void SignIn(string userName, bool stayLoggedIn = true);

    void SignOut();
  }
}