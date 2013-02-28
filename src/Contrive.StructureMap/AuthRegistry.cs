using Contrive.Auth;
using Contrive.Auth.Membership;
using Contrive.Auth.Web.Membership;
using Contrive.Common;
using Microsoft.Practices.ServiceLocation;
using StructureMap.Configuration.DSL;

namespace Contrive.StructureMap
{
    public class AuthRegistry : Registry
    {
        public AuthRegistry()
        {
            For<IMembershipConfigurationProvider>().Singleton().Use<MembershipConfigurationProvider>();
            For<UserServiceExtended>().Singleton();
            Forward<UserServiceExtended, IUserService>();
            Forward<UserServiceExtended, IUserServiceExtended>();
            For<IRoleService>().Singleton().Use<RoleService>();
            For<ISecurityService>().Singleton().Use<SecurityService>();

            var sl = ServiceLocator.Current;
            var cryptoConfig = sl.GetInstance<ICryptoConfigurationProvider>();
            var cryptographer = new Cryptographer(cryptoConfig.EncryptionKey, cryptoConfig.EncryptionAlgorithm, cryptoConfig.HmacKey,
                                                  cryptoConfig.HashAlgorithm);
            For<ICryptographer>().Singleton().Use(cryptographer);

            var userServiceConfig = sl.GetInstance<IMembershipConfigurationProvider>().UserServiceConfiguration;
            var userServiceSettings = new UserServiceSettings(userServiceConfig);
            For<IUserServiceSettings>().Singleton().Use(userServiceSettings);
        }
    }
}