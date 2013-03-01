using System.Data.Entity;
using Contrive.Auth;
using Contrive.Auth.EntityFramework;
using Contrive.Common;
using StructureMap.Configuration.DSL;

namespace Contrive.StructureMap
{
    public class AuthEntityFrameworkRegistry : Registry
    {
        public AuthEntityFrameworkRegistry()
        {
            For<DbContext>().HybridHttpOrThreadLocalScoped().Use<ContriveContext>();
            For(typeof (IRepository<>)).HybridHttpOrThreadLocalScoped().Use(typeof (Repository<>));
            For<IUserRepository>().Singleton().Use<UserRepository>();
        }
    }
}