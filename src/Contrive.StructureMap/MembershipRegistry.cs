using Contrive.Auth.Membership;
using Contrive.Auth.Web.Membership;
using StructureMap.Configuration.DSL;

namespace Contrive.StructureMap
{
    public class MembershipRegistry : Registry
    {
        public MembershipRegistry()
        {
            For<IRoleServiceExtended>().Singleton().Use<RoleServiceExtended>();
            For<ISecurityService>().Singleton().Use<SecurityService>();
        }
    }
}