using System;
using System.Collections.Generic;
using System.Linq;
using Contrive.Core;
using Contrive.Core.Extensions;

namespace Contrive.EntityFramework
{
  public class RoleRepository : IRoleRepository
  {
    readonly Repository<Role> _repository;

    public RoleRepository(Repository<Role> repository)
    {
      _repository = repository;
    }

    public IQueryable<IRole> GetQuery()
    {
      return _repository.GetQuery();
    }

    public IEnumerable<IRole> GetAll()
    {
      return _repository.GetAll();
    }

    public IEnumerable<IRole> Where(Func<IRole, bool> @where)
    {
      return _repository.Where(@where);
    }

    public IRole FirstOrDefault(Func<IRole, bool> @where)
    {
      return _repository.FirstOrDefault(@where);
    }

    public void Insert(IRole role)
    {
      _repository.Insert(role.As<Role>());
    }

    public void Update(IRole role)
    {
      _repository.Update(role.As<Role>());
    }

    public void Delete(IRole role)
    {
      _repository.Delete(role.As<Role>());
    }

    public void SaveChanges()
    {
      _repository.SaveChanges();
    }
  }
}