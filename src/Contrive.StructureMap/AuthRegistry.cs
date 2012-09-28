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
      For<ICryptographer>().Singleton().Use<Cryptographer>();
      For<IUserServiceSettings>().Singleton().Use(c => c.GetInstance<IAuthConfigurationProvider>().UserServiceSettings);
    }
  }
}