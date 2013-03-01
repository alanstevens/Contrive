using System.Collections.Specialized;

namespace Contrive.Auth.Membership
{
    public interface IMembershipConfigurationProvider : IAuthConfigurationProvider
    {
        NameValueCollection RoleServiceConfiguration { get; }
    }
}