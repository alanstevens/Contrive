using System;
using Contrive.Common;

namespace Contrive.Data.Common
{
  public class ConnectionStringProvider : IStartupTask
  {
    readonly IConfigurationProvider _configurationProvider;
    public static Func<string> GetConnectionString = () => "";

    public ConnectionStringProvider(IConfigurationProvider configurationProvider)
    {
      _configurationProvider = configurationProvider;
    }

    public void OnStartup()
    {
      GetConnectionString = () => _configurationProvider.DefaultConnectionString;
    }
  }
}