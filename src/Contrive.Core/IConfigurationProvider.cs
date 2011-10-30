using System.Collections.Specialized;

namespace Contrive.Core
{
  public interface IConfigurationProvider
  {
    NameValueCollection AppSettings { get; }

    NameValueCollection RoleServiceConfiguration { get; }

    IUserServiceSettings UserServiceSettings { get; }

    T GetSection<T>(string sectionName);

    string GetMachineKey();

    string GetDecryptionAlgorithm();
  }
}