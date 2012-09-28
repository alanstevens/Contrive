using System.Data.Entity;
using Contrive.Auth;
using Contrive.Auth.EntityFramework;
using Contrive.Common;
using Contrive.EntityFramework;
using StructureMap.Configuration.DSL;

namespace Contrive.StructureMap
{
  public class AuthEntityFrameworkRegistry : Registry
  {
    public AuthEntityFrameworkRegistry()
    {
      For<DbContext>().HybridHttpOrThreadLocalScoped().Use<CointriveContext>();
      For(typeof (IRepository<>)).HybridHttpOrThreadLocalScoped().Use(typeof (Repository<>));
      For<IUserRepository>().Singleton().Use<UserRepository>();
      For<IRoleRepository>().Singleton().Use<RoleRepository>();
      For<IRole>().Use<Role>();
      For<IUser>().Use<User>();
    }
  }
}