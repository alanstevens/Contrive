using System.Collections.Specialized;
using Contrive.Common;

namespace Contrive.Auth.Membership
{
  public interface IMembershipConfigurationProvider : IConfigurationProvider
  {
    NameValueCollection RoleServiceConfiguration { get; }

    NameValueCollection UserServiceConfiguration { get; }
  }
}