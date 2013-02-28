using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;

namespace Contrive.Common
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        public NameValueCollection AppSettings
        {
            [DebuggerStepThrough] get { return ConfigurationManager.AppSettings; }
        }

        public string DefaultConnectionString
        {
            [DebuggerStepThrough] get { return GetConnectionString("default"); }
        }

        public string DefaultConnectionProvider
        {
            [DebuggerStepThrough] get { return GetConnectionProvider("default"); }
        }

        [DebuggerStepThrough]
        public string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        [DebuggerStepThrough]
        public string GetConnectionProvider(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ProviderName;
        }

        [DebuggerStepThrough]
        public T GetSection<T>(string sectionName)
        {
            return (T) ConfigurationManager.GetSection(sectionName);
        }
    }
}