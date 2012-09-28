using System.Collections.Specialized;
using System.Configuration;

namespace Contrive.Common
{
  public class ConfigurationProvider : IConfigurationProvider
  {
    public NameValueCollection AppSettings { get { return ConfigurationManager.AppSettings; } }

    public string DefaultConnectionString { get { return GetConnectionString("default"); } }

    public string DefaultConnectionProvider { get { return GetConnectionProvider("default"); } }

    public string GetConnectionString(string name)
    {
      return ConfigurationManager.ConnectionStrings[name].ConnectionString;
    }

    public string GetConnectionProvider(string name)
    {
      return ConfigurationManager.ConnectionStrings[name].ProviderName;
    }

    public T GetSection<T>(string sectionName)
    {
      return (T) ConfigurationManager.GetSection(sectionName);
    }
  }
}