namespace Contrive.Auth
{
  /// <summary>
  /// Anti-corruption interface to wrap native/legacy auth systems.
  /// </summary>
  public interface IPlatformAuthenticationService
  {
    bool SignIn(string userName, string password, bool rememberMe = false);

    void SignOut();
  }
}