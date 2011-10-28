using System.Data.Entity;
using Contrive.Core;

namespace Contrive.EntityFramework
{
  public class RoleRepository : Repository<IRole>, IRoleRepository
  {
    public RoleRepository(DbContext context) : base(context) { }
  }
}