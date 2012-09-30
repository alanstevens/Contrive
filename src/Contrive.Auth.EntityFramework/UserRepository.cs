using System;
using System.Collections.Generic;
using System.Linq;
using Contrive.Common.Extensions;

namespace Contrive.Auth.EntityFramework
{
  public class UserRepository : IUserRepository
  {
    public UserRepository(Repository<User> repository)
    {
      _repository = repository;
    }

    readonly Repository<User> _repository;

    public IEnumerable<IUser> GetAll()
    {
      return _repository.GetAll();
    }

    public IUser GetUserByUserName(string userName)
    {
      return _repository.FirstOrDefault(u => u.UserName == userName);
    }

    public IUser GetUserByEmailAddress(string emailAddress)
    {
      return _repository.FirstOrDefault(u => u.Email == emailAddress);
    }

    public IEnumerable<IUser> FindUsersForUserName(string searchTerm)
    {
      return _repository.Where(u => u.UserName.Contains(searchTerm));
    }

    public IEnumerable<IUser> FindUsersForEmailAddress(string searchTerm)
    {
      return _repository.Where(u => u.Email.Contains(searchTerm));
    }

    public IEnumerable<IUser> GetUsersForUserNames(IEnumerable<string> userNames)
    {
      return _repository.Where(u => userNames.Contains(u.UserName));
    }

    public IUser GetUserByConfirmationToken(string token)
    {
      return _repository.FirstOrDefault(u => u.ConfirmationToken == token);
    }

    public IUser GetUserByPasswordVerificationToken(string token)
    {
      return _repository.FirstOrDefault(user => String.Equals(user.PasswordVerificationToken, token, StringComparison.OrdinalIgnoreCase) && user.PasswordVerificationTokenExpirationDate > DateTime.UtcNow);
    }

    public void Insert(IUser user)
    {
      _repository.Insert(user.As<User>());
      SaveChanges();
    }

    public void Update(IUser user)
    {
      _repository.Update(user.As<User>());
      SaveChanges();
    }

    public void Delete(IUser user)
    {
      _repository.Delete(user.As<User>());
      SaveChanges();
    }

    void SaveChanges()
    {
      _repository.SaveChanges();
    }
  }
}