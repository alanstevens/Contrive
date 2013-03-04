using System.Collections.Specialized;
using System.Diagnostics;
using System.Web.Configuration;
using Contrive.Auth.Membership;

namespace Contrive.Auth.Web.Membership
{
    public class AuthConfigurationProvider : Web.AuthConfigurationProvider, IAuthConfigurationProvider
    {
        public NameValueCollection RoleServiceConfiguration
        {
            [DebuggerStepThrough]
            get
            {
                var roleManagerSection = GetSection<RoleManagerSection>("system.web/roleManager");
                var defaultProvider = roleManagerSection.DefaultProvider;
                return roleManagerSection.Providers[defaultProvider].Parameters;
            }
        }
    }
}