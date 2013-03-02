using System.Collections.Generic;
using System.Security.Principal;
using Contrive.Auth;

namespace Contrive.Web.Common
{
    public interface IUserService
    {
        IUser CurrentUser { get; }

        void UpdateCurrentUserWith(IPrincipal principal);

        void UpdateCurrentUserWith(IUser user, Dictionary<string, string> userCapabilities = null);
    }
}