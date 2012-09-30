using System;
using System.IO;
using System.Reflection;
using Contrive.Common.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Common
{
  // Called like so:
  // Bootstrapper.Configurator = ContriveStructureMapConfigurator.Configure;
  // Bootstrapper.Run("Contrive");
  public static class Bootstrapper
  {
    public static Action<string, string> Configurator = (assemblyDirectory, rootNamespace) => { };
    public static Action BeforeStartup = () => { };
    public static Action AfterStartup = () => { };

    static string AssemblyDirectory
    {
      get
      {
        var codeBase = Assembly.GetExecutingAssembly().CodeBase;
        var uri = new UriBuilder(codeBase);
        var path = Uri.UnescapeDataString(uri.Path);
        return Path.GetDirectoryName(path);
      }
    }

    public static void Run(string rootNamespace)
    {
      Configurator.Invoke(AssemblyDirectory, rootNamespace);

      BeforeStartup();

      // run the startup tasks
      ServiceLocator.Current.GetAllInstances<IStartupTask>().Each(x => x.OnStartup());

      // Listeners are singletons scoped to the application lifespan
      ServiceLocator.Current.GetAllInstances<IListener>();

      AfterStartup();
    }
  }
}