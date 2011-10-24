using Contrive.Core;
using Contrive.Web.Configuration;
using Microsoft.Practices.ServiceLocation;
using StructureMap;
using StructureMap.ServiceLocatorAdapter;

namespace Contrive.StructureMap
{
  public static class Configurator
  {
    public static void ConfigureWith(IContainer container)
    {
      var serviceLocator = new StructureMapServiceLocator(container);
      ServiceLocator.SetLocatorProvider(() => serviceLocator);
      container.Configure(x =>
      {
        x.For<IServiceLocator>().Singleton().Use(serviceLocator);
        x.For<IConfigurationProvider>().Singleton().Use<ConfigurationProvider>();
        x.For<IUserProvider>().Singleton().Use<UserProvider>();
        x.For<IRoleProvider>().Singleton().Use<RoleProvider>();
        x.For<ICryptographer>().Singleton().Use<Cryptographer>();
      });
    }
  }
}