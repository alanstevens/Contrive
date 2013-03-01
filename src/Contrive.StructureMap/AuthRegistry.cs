using Contrive.Auth;
using Contrive.Auth.Membership;
using Contrive.Auth.Web.Membership;
using Contrive.Common;
using StructureMap.Configuration.DSL;

namespace Contrive.StructureMap
{
    public class AuthRegistry : Registry
    {
        public AuthRegistry()
        {
            For<IAuthConfigurationProvider>().Singleton().Use<IAuthConfigurationProvider>();
            For<IUserService>().Singleton().Use<UserService>();
            For<IRoleService>().Singleton().Use<RoleService>();
            For<ISecurityService>().Singleton().Use<SecurityService>();
            For<ICryptographer>().Singleton().Use(s =>
                                                  {
                                                      var cryptoConfig = s.GetInstance<ICryptoConfigurationProvider>();
                                                      return new Cryptographer(cryptoConfig.EncryptionKey,
                                                                               cryptoConfig.EncryptionAlgorithm,
                                                                               cryptoConfig.HmacKey,
                                                                               cryptoConfig.HashAlgorithm);
                                                  });
            For<IUserServiceSettings>().Singleton()
                                       .Use(s => new UserServiceSettings(s.GetInstance<IAuthConfigurationProvider>().UserServiceConfiguration));
        }
    }
}