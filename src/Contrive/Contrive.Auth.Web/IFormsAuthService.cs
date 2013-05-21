using System.Security.Principal;
using System.Web.Security;

namespace Contrive.Auth.Web
{
    public interface IFormsAuthService
    {
        IPrincipal GetUpdatedPrincipalFor(IPrincipal principal);

        IPrincipal GetUpdatedPrincipalFor(IUser user, bool stayLoggedIn = false, FormsAuthenticationTicket currentTicket = null);
    }
}