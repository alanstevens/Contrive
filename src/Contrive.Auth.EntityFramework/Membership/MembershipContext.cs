using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using Contrive.Auth.Membership;

namespace Contrive.Auth.EntityFramework.Membership
{
    public class MembershipContext : DbContext
    {
        public DbSet<UserExtended> Users { get; set; }
        public DbSet<RoleExtended> Roles { get; set; }

        public ObjectContext ObjectContext()
        {
            return (this as IObjectContextAdapter).ObjectContext;
        }
    }
}