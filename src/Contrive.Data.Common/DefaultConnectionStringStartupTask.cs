using Contrive.Common;

namespace Contrive.Data.Common
{
  public class DefaultConnectionStringStartupTask : IStartupTask
  {
    public DefaultConnectionStringStartupTask(IConfigurationProvider configurationProvider)
    {
      _configurationProvider = configurationProvider;
    }

    readonly IConfigurationProvider _configurationProvider;

    public void OnStartup()
    {
      ConnectionStringProvider.GetConnectionString = () => _configurationProvider.DefaultConnectionString;
    }
  }
}