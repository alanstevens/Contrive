using System.Collections.Specialized;
using Contrive.Common;

namespace Contrive.Auth
{
  public interface IAuthConfigurationProvider : IConfigurationProvider
  {
    NameValueCollection RoleServiceConfiguration { get; }

    NameValueCollection UserServiceConfiguration { get; }

    string DecryptionKey { get; }

    string DecryptionAlgorithm { get; }
  }
}