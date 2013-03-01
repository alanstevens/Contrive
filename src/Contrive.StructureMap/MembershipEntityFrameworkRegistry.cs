using System.Data.Entity;
using Contrive.Auth.EntityFramework;
using Contrive.Auth.EntityFramework.Membership;
using Contrive.Auth.Membership;
using Contrive.Common;
using StructureMap.Configuration.DSL;

namespace Contrive.StructureMap
{
    public class MembershipEntityFrameworkRegistry : Registry
    {
        public MembershipEntityFrameworkRegistry()
        {
            For<DbContext>().HybridHttpOrThreadLocalScoped().Use<MembershipContext>();
            For(typeof (IRepository<>)).HybridHttpOrThreadLocalScoped().Use(typeof (Repository<>));
            For<IUserExtendedRepository>().Singleton().Use<UserExtendedRepository>();
            For<IRoleRepository>().Singleton().Use<RoleRepository>();
            For<IRole>().Use<Role>();
        }
    }
}