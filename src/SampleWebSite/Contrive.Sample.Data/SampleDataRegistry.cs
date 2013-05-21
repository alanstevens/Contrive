using Contrive.Auth;
using StructureMap.Configuration.DSL;

namespace Contrive.Sample.Data
{
    public class SampleDataRegistry : Registry
    {
        public SampleDataRegistry()
        {
            //For<IUserRepository>()
            //    .HttpContextScoped()
            //    .Use<UserRepository>();

            //For<IUserRepository>()
            //    .Use<UserRepository>();

            For<IRoleRepository>()
                .HttpContextScoped()
                .Use<RoleRepository>();
        }
    }
}