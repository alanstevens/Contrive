using System;
using System.Collections.Generic;
using System.Linq;
using Contrive.Auth.Membership;
using Contrive.Common;
using Contrive.Common.Extensions;

namespace Contrive.Auth.EntityFramework
{
  public class UserExtendedRepository : IUserExtendedRepository
  {
    public UserExtendedRepository(IRepository<IUserExtended> repository)
    {
      _repository = repository;
    }

    readonly IRepository<IUserExtended> _repository;

    public IEnumerable<IUserExtended> GetAll()
    {
      return _repository.GetAll();
    }

    public IUserExtended GetUserByUserName(string userName)
    {
      return _repository.FirstOrDefault(u => u.UserName == userName);
    }

    public IUserExtended GetUserByEmailAddress(string emailAddress)
    {
      return _repository.FirstOrDefault(u => u.Email == emailAddress);
    }

    public IEnumerable<IUserExtended> FindUsersForUserName(string searchTerm)
    {
      return _repository.Where(u => u.UserName.Contains(searchTerm));
    }

    public IEnumerable<IUserExtended> FindUsersForEmailAddress(string searchTerm)
    {
      return _repository.Where(u => u.Email.Contains(searchTerm));
    }

    public IEnumerable<IUserExtended> GetUsersForUserNames(IEnumerable<string> userNames)
    {
      return _repository.Where(u => userNames.Contains(u.UserName));
    }

    public IUserExtended GetUserByConfirmationToken(string token)
    {
      return _repository.FirstOrDefault(u => u.ConfirmationToken == token);
    }

    public IUserExtended GetUserByPasswordVerificationToken(string token)
    {
      return
        _repository.FirstOrDefault(
                                   user =>
                                   String.Equals(user.PasswordVerificationToken, token,
                                                 StringComparison.OrdinalIgnoreCase) &&
                                   user.PasswordVerificationTokenExpirationDate > DateTime.UtcNow);
    }

    public void Insert(IUserExtended user)
    {
      _repository.Insert(user.As<User>());
      SaveChanges();
    }

    public void Update(IUserExtended user)
    {
      _repository.Update(user.As<User>());
      SaveChanges();
    }

    public void Delete(IUserExtended user)
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