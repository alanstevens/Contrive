using System.Data.Entity;
using Contrive.Core;

namespace Contrive.EntityFramework
{
  public class UserRepository : Repository<IUser>, IUserRepository
  {
    public UserRepository(DbContext context) : base(context) { }
  }
}