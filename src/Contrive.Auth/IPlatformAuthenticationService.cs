using System.Security.Principal;

namespace Contrive.Auth
{
  /// <summary>
  ///   Anti-corruption interface to wrap native/legacy auth systems.
  /// </summary>
  public interface IPlatformAuthenticationService
  {
    bool UserIsAuthenticated { get; }

    IPrincipal CurrentPrincipal { get; }

    bool SignIn(string userName, string password, bool rememberMe = false);

    void SignOut();

    void Unauthorize();
  }
}