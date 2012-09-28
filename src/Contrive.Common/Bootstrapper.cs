using System;
using System.IO;
using System.Reflection;
using Contrive.Common.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Common
{
  public static class Bootstrapper
  {
    static string BinDirectory
    {
      get
      {
        var codeBase = Assembly.GetExecutingAssembly().CodeBase;
        var uri = new UriBuilder(codeBase);
        var path = Uri.UnescapeDataString(uri.Path);
        return Path.GetDirectoryName(path);
      }
    }

    public static void Run(Action<string, string> configurator = null, string rootNamespace = null)
    {
      if (configurator.IsNull()) return;

      configurator.Invoke(BinDirectory, rootNamespace);

      // run the startup tasks
      ServiceLocator.Current.GetAllInstances<IStartupTask>().Each(x => x.OnStartup());

      // Listeners are singletons scoped to the application lifespan
      ServiceLocator.Current.GetAllInstances<IListener>();
    }
  }
}