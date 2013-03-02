using Contrive.Auth.Membership;
using Contrive.Auth.Web.Membership;
using StructureMap.Configuration.DSL;

namespace Contrive.StructureMap
{
    public class MembershipRegistry : Registry
    {
        public MembershipRegistry()
        {
            For<IRoleService>().Singleton().Use<RoleService>();
            For<ISecurityService>().Singleton().Use<SecurityService>();
        }
    }
}