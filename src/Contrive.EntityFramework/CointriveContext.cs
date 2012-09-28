using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;

namespace Contrive.EntityFramework
{
  public class CointriveContext : DbContext
  {
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    public ObjectContext ObjectContext()
    {
      return (this as IObjectContextAdapter).ObjectContext;
    }
  }
}