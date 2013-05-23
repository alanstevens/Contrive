using Contrive.Auth;
using StructureMap.Configuration.DSL;

namespace Contrive.Sample.Data
{
    public class SampleDataRegistry : Registry
    {
        public SampleDataRegistry()
        {
            For<IUserRepository>()
                .HybridHttpOrThreadLocalScoped()
                .Use<UserRepository>();

            For<IRoleRepository>()
                .HybridHttpOrThreadLocalScoped()
                .Use<RoleRepository>();
        }
    }
}