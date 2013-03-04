using System.Security.Principal;

namespace Contrive.Auth
{
    /// <summary>
    ///     Anti-corruption interface to wrap native/legacy auth systems.
    /// </summary>
    public interface IPlatformAuthenticationService
    {
        bool UserIsAuthenticated { get; }

        IPrincipal CurrentPrincipal { get; }

        bool SignIn(IUser user, bool rememberMe = false);

        void SignOut();

        void Deauthorize();
    }
}