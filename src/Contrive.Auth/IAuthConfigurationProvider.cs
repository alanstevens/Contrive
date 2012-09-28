using System.Collections.Specialized;
using Contrive.Common;

namespace Contrive.Auth
{
  public interface IAuthConfigurationProvider: IConfigurationProvider
  {
    NameValueCollection RoleServiceConfiguration { get; }

    IUserServiceSettings UserServiceSettings { get; }

    string GetMachineKey();

    string GetDecryptionAlgorithm();
  }
}