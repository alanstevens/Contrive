using System.Collections.Specialized;

namespace Contrive.Core
{
  public interface IConfigurationProvider
  {
    NameValueCollection AppSettings { get; }

    NameValueCollection UserServiceConfiguration { get; }

    NameValueCollection RoleServiceConfiguration { get; }

    T GetSection<T>(string sectionName);

    string GetMachineKey();

    string GetDecryptionAlgorithm();
  }
}