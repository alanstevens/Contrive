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
      For<IMembershipConfigurationProvider>().Singleton().Use<MembershipConfigurationProvider>();
      For<UserService>().Singleton();
      Forward<UserService, IUserService>();
      Forward<UserService, IUserServiceExtended>();
      For<IRoleService>().Singleton().Use<RoleService>();
      For<ISecurityService>().Singleton().Use<SecurityService>();
      For<ICryptographer>().Singleton().Use(c =>
                                            {
                                              var config = c.GetInstance<ICryptoConfigurationProvider>();
                                              return new Cryptographer(config.EncryptionKey, config.EncryptionAlgorithm, config.HmacKey, config.HashAlgorithm);
                                            });
      For<IUserServiceSettings>().Singleton().Use(
                                                  c =>
                                                  new UserServiceSettings(
                                                    c.GetInstance<IMembershipConfigurationProvider>().UserServiceConfiguration));
    }
  }
}