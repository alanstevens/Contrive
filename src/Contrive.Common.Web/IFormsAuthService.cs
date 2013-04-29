using System.Collections.Generic;
using System.Security.Principal;
using Contrive.Auth;

namespace Contrive.Common.Web
{
    public interface IFormsAuthService
    {
        IUser CurrentUser { get; }

        void UpdateCurrentUserWith(IPrincipal principal);

        void UpdateCurrentUserWith(IUser user, bool stayLoggedIn = false, IEnumerable<string> roleNames = null);
    }
}