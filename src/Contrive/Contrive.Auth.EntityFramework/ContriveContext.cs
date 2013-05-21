using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;

namespace Contrive.Auth.EntityFramework
{
    public class ContriveContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public ObjectContext ObjectContext()
        {
            return (this as IObjectContextAdapter).ObjectContext;
        }
    }
}