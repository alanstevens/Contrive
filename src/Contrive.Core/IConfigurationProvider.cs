using System.Collections.Specialized;

namespace Contrive.Core
{
  public interface IConfigurationProvider
  {
    NameValueCollection AppSettings { get; }

    NameValueCollection UserManagerConfiguration { get; }

    NameValueCollection RoleManagerConfiguration { get; }

    T GetSection<T>(string sectionName);

    string GetMachineKey();

    string GetDecryptionAlgorithm();
  }
}