using System.Collections.Specialized;
using Contrive.Common;

namespace Contrive.Auth
{
  public interface IMembershipConfigurationProvider : IConfigurationProvider
  {
    NameValueCollection RoleServiceConfiguration { get; }

    NameValueCollection UserServiceConfiguration { get; }
  }
}