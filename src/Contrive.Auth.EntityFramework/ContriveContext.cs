using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using Contrive.Auth.Membership;

namespace Contrive.Auth.EntityFramework
{
    public class ContriveContext : DbContext
    {
        public DbSet<UserExtended> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public ObjectContext ObjectContext()
        {
            return (this as IObjectContextAdapter).ObjectContext;
        }
    }
}