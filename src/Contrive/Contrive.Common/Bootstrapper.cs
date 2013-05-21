using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Contrive.Common.Extensions;
using Microsoft.Practices.ServiceLocation;
using log4net;

namespace Contrive.Common
{
    // Called like so:
    // Bootstrapper.Configurator = StructureMapConfigurator.ConfigureWith;
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
            BeforeStartup();

            try
            {
                var configurator = Configurator;
                configurator.Invoke(AssemblyDirectory, rootNamespace);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
                // a container failure probably means no IoC, so we fall back to basic object usage
                LogManager.GetLogger(typeof (Bootstrapper).Name).Fatal(ex);
            }

            // run the startup tasks
            ServiceLocator.Current.GetAllInstances<IStartupTask>().Each(x => x.OnStartup());

            // Listeners are singletons scoped to the application lifespan
            ServiceLocator.Current.GetAllInstances<IListener>();

            AfterStartup();
        }
    }
}