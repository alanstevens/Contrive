using Contrive.Auth;
using Contrive.Auth.Web;
using Contrive.Common;
using Contrive.Common.Extensions;
using StructureMap.Configuration.DSL;

namespace Contrive.StructureMap
{
    public class AuthRegistry : Registry
    {
        public AuthRegistry()
        {
            For<IAuthConfigurationProvider>().Singleton().Use<AuthConfigurationProvider>();
            For<IUserService>().Singleton().Use<UserService>();
            For<ICryptographer>().Singleton().Use(s =>
                                                  {
                                                      var cryptoConfig = s.GetInstance<ICryptoConfigurationProvider>();
                                                      return BuildCryptographer(cryptoConfig);
                                                  });
            For<IUserServiceSettings>().Singleton()
                                       .Use(s => new UserServiceSettings(s.GetInstance<IAuthConfigurationProvider>().UserServiceConfiguration));
        }

        static ICryptographer BuildCryptographer(ICryptoConfigurationProvider cryptoConfig)
        {
            var encryptionAlgorithm = cryptoConfig.EncryptionAlgorithm;
            var hashAlgorithm = cryptoConfig.HashAlgorithm;
            var encryptionKey = cryptoConfig.EncryptionKey;
            var hmacKey = cryptoConfig.HmacKey;
            var generic = typeof (Cryptographer<,>);
            var specific = generic.MakeGenericType(new[] {encryptionAlgorithm, hashAlgorithm});
            var ci = specific.GetConstructor(new[] {typeof (byte[]), typeof (byte[])});
            var o = ci.Invoke(new object[] {encryptionKey, hmacKey});
            return o.As<ICryptographer>();
        }
    }
}