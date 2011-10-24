using System;
using System.Collections.Generic;

namespace Contrive.Core
{
  public interface IUserRepository
  {
    IUser FirstOrDefault(Func<IUser, bool> where);

    IEnumerable<IUser> Where(Func<IUser, bool> where);

    void Insert(IUser user);

    void Update(IUser user);

    void Delete(IUser user);

    void SaveChanges();
  }
}