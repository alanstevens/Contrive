using System.Collections.Specialized;

namespace Contrive.Common
{
  public interface IConfigurationProvider
  {
    NameValueCollection AppSettings { get; }

    string DefaultConnectionString { get; }

    string DefaultConnectionProvider { get; }

    string GetConnectionString(string name);

    string GetConnectionProvider(string name);

    T GetSection<T>(string sectionName);
  }
}