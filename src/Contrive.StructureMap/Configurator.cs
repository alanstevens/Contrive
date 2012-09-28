using Contrive.Core;
using Contrive.Web;
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
                            x.For<IUserService>().Singleton().Use<UserService>();
                            x.For<IRoleService>().Singleton().Use<RoleService>();
                            x.For<ICryptographer>().Singleton().Use<Cryptographer>();
                            x.For<IUserServiceSettings>().Singleton().Use(c => c.GetInstance<IConfigurationProvider>().UserServiceSettings);
                          });
    }
  }
}