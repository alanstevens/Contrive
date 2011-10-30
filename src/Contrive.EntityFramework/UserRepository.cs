using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Contrive.Core;
using Contrive.Core.Extensions;

namespace Contrive.EntityFramework
{
  public class UserRepository : IUserRepository
  {
    readonly Repository<User> _repository;

    public UserRepository(Repository<User> repository)
    {
      _repository = repository;
    }

    public IUser FirstOrDefault(Func<IUser, bool> @where)
    {
      return _repository.FirstOrDefault(@where);
    }

    public IEnumerable<IUser> Where(Func<IUser, bool> @where)
    {
      return _repository.Where(@where);
    }

    public void Insert(IUser user)
    {
      _repository.Insert(user.As<User>());
    }

    public void Update(IUser user)
    {
      _repository.Update(user.As<User>());
    }

    public void Delete(IUser user)
    {
      _repository.Delete(user.As<User>());
    }

    public void SaveChanges()
    {
      _repository.SaveChanges();
    }
  }
}