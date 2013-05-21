using System.Diagnostics;
using Contrive.Auth;
using Contrive.Auth.Web;
using Contrive.Common;
using Contrive.Common.Extensions;
using Contrive.Common.Web;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace Contrive.StructureMap
{
    public class AuthRegistry : Registry
    {
        public AuthRegistry()
        {
            For<IAuthConfigurationProvider>()
                .Singleton()
                .Use<AuthConfigurationProvider>();
            For<ICryptographer>()
                .Singleton()
                .Use(BuildCryptographer);
            For<IUserService>()
                .HybridHttpOrThreadLocalScoped()
                .Use<UserService>();
            For<IRoleService>()
                .HybridHttpOrThreadLocalScoped()
                .Use<RoleService>();
            For<ILogOnService>()
                .HybridHttpOrThreadLocalScoped()
                .Use<LogOnService>();
            For<IUserServiceSettings>()
                .Singleton()
                .Use(s => new UserServiceSettings(s.GetInstance<IAuthConfigurationProvider>()
                                                   .UserServiceConfiguration));
        }

        static ICryptographer BuildCryptographer(IContext context)
        {
            var processName = Process.GetCurrentProcess()
                                     .ProcessName.ToLower();
            if (processName.Contains("w3wp") || processName.Contains("aspnet_wp") || processName.Contains("iisexpress"))
                return new WebCryptographer();

            var cryptoConfig = context.GetInstance<ICryptoConfigurationProvider>();
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