using System;
using System.Collections.Specialized;
using Contrive.Common;

namespace Contrive.Auth
{
  public interface IAuthConfigurationProvider : IConfigurationProvider
  {
    NameValueCollection RoleServiceConfiguration { get; }

    NameValueCollection UserServiceConfiguration { get; }

    Type DecryptionAlgorithm { get; }

    byte[] DecryptionKey { get; }

    Type ValidationAlgorithm { get; }

    byte[] ValidationKey { get; }
  }
}