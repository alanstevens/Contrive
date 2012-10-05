using Contrive.Auth;
using Contrive.Auth.Web;
using Contrive.Common;
using StructureMap.Configuration.DSL;

namespace Contrive.StructureMap
{
  public class AuthRegistry : Registry
  {
    public AuthRegistry()
    {
      For<IAuthConfigurationProvider>().Singleton().Use<WebAuthConfigurationProvider>();
      For<UserService>().Singleton();
      Forward<UserService, IUserService>();
      Forward<UserService, IUserServiceExtended>();
      For<IRoleService>().Singleton().Use<RoleService>();
      For<ISecurityService>().Singleton().Use<SecurityService>();
      For<ICryptographer>().Singleton().Use(c =>
                                            {
                                              var authConfigurationProvider = c.GetInstance<IAuthConfigurationProvider>();
                                              return new Cryptographer(authConfigurationProvider.DecryptionKey, authConfigurationProvider.DecryptionAlgorithm);
                                            });
      For<IUserServiceSettings>().Singleton().Use(c => new UserServiceSettings(c.GetInstance<IAuthConfigurationProvider>().UserServiceConfiguration));
    }
  }
}