using System;
using System.Reflection;
using Contrive.Common.Extensions;
using Microsoft.Practices.ServiceLocation;
using StructureMap;
using StructureMap.Graph;
using StructureMap.ServiceLocatorAdapter;

namespace Contrive.StructureMap
{
  public static class StructureMapContainerConfigurator
  {
    public static bool Configured { get; private set; }

    public static void ConfigureWith(string assemblyDirectory,
                                     string rootNamespace,
                                     Action<IAssemblyScanner> customScanner = null,
                                     Action<ConfigurationExpression> interceptors = null)
    {
      if (Configured) return;

      if (customScanner.IsNull()) customScanner = s => { };

      if (interceptors.IsNull()) interceptors = x => { };

      var container = new Container();

      var serviceLocator = new StructureMapServiceLocator(container);
      ServiceLocator.SetLocatorProvider(() => serviceLocator);

      container.Configure(x =>
                          {
                            x.For<IContainer>().Use(container);
                            x.For<IServiceLocator>().Singleton().Use(serviceLocator);
                            interceptors(x);
                            x.Scan(s =>
                                   {
                                     s.Assembly(Assembly.GetEntryAssembly());
                                     s.Assembly(Assembly.GetExecutingAssembly());
                                     if (assemblyDirectory.IsNotBlank() && rootNamespace.IsNotBlank())
                                       s.AssembliesFromPath(assemblyDirectory,
                                                            assembly =>
                                                            assembly.GetName().Name.StartsWith(rootNamespace));
                                     s.WithDefaultConventions();
                                     s.LookForRegistries();
                                     customScanner(s);
                                   });
                          });

      Configured = true;
    }
  }
}