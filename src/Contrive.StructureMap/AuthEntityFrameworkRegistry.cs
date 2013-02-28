using System.Data.Entity;
using Contrive.Auth.EntityFramework;
using Contrive.Auth.Membership;
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
            For<IUserExtendedRepository>().Singleton().Use<UserExtendedRepository>();
            For<IRoleRepository>().Singleton().Use<RoleRepository>();
            For<IRole>().Use<Role>();
            For<IUserExtended>().Use<UserExtended>();
        }
    }
}