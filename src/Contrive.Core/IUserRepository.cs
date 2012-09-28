using System;
using System.Collections.Generic;

namespace Contrive.Core
{
  public interface IUserRepository
  {
    IEnumerable<IUser> GetAll();

    IUser GetUserByUserName(string userName);

    IUser GetUserByEmailAddress(string emailAddress);

    IEnumerable<IUser> FindUsersForUserName(string searchTerm);

    IEnumerable<IUser> FindUsersForEmailAddress(string searchTerm);

    IEnumerable<IUser> GetUsersForUserName(IEnumerable<string> userNames);

    IUser GetUserByConfirmationToken(string token);

    IUser GetUserByPasswordVerificationToken(string token);

    void Insert(IUser user);

    void Update(IUser user);

    void Delete(IUser user);
  }
}