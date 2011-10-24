using System;
using System.Collections.Generic;
using System.Linq;

namespace Contrive.Core
{
  public interface IRoleRepository
  {
    IQueryable<IRole> GetQuery();

    IEnumerable<IRole> GetAll();

    IEnumerable<IRole> Where(Func<IRole, bool> where);

    IRole FirstOrDefault(Func<IRole, bool> where);

    void Insert(IRole role);

    void Update(IRole role);

    void Delete(IRole role);

    void SaveChanges();
  }
}