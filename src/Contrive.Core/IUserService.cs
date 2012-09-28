using System;
using System.Collections.Generic;

namespace Contrive.Core
{
  public interface IUserService
  {
    IUserServiceSettings Settings { get; }

    bool ChangePassword(string userName, string oldPassword, string newPassword);

    IUser GetUserByUserName(string userName);

    bool ValidateUser(string userName, string password);

    UserCreateStatus CreateUser(string userName, string password, string email, bool isApproved);

    bool VerifyUser(IUser user, string password);

    UserCreateStatus CreateUser(string userName, string password, string email);

    string CreateAccount(string userName, string password, string email, bool requireConfirmationToken = false);

    bool ConfirmAccount(string token);

    bool DeleteAccount(string userName);

    bool IsConfirmed(string userName);

    string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow);

    bool ResetPasswordWithToken(string token, string newPassword);

    string ValidateUserExtended(string userNameOrEmail, string password);

    DateTime GetPasswordChangedDate(string userName);

    DateTime GetCreateDate(string userName);

    int GetPasswordFailuresSinceLastSuccess(string userName);

    IUser GetUserFromPasswordResetToken(string token);

    DateTime GetLastPasswordFailureDate(string userName);

    IUser GetUserByEmail(string emailAddress);

    void UpdateUser(IUser user);

    IUser GetUserByUserNameOrEmail(string userNameOrEmail);

    IEnumerable<IUser> FindUsersForUserName(string searchTerm);

    IEnumerable<IUser> FindUsersForEmail(string searchTerm);
  }
}