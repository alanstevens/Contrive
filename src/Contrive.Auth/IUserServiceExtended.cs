using System;

namespace Contrive.Auth
{
  public interface IUserServiceExtended : IUserService
  {
    bool VerifyUser(IUser user, string password);

    string CreateAccount(string userName, string password, string emailAddress, bool requireConfirmationToken = false);

    bool ConfirmAccount(string token);

    bool IsConfirmed(string userName);

    string ValidateUserExtended(string userNameOrEmailAddress, string password);

    DateTime GetPasswordChangedDate(string userName);

    DateTime GetCreateDate(string userName);

    int GetPasswordFailuresSinceLastSuccess(string userName);

    DateTime GetLastPasswordFailureDate(string userName);

    IUser GetUserByUserNameOrEmailAddress(string userNameOrEmailAddress);
  }
}