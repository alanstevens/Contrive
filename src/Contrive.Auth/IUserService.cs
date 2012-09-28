using System.Collections.Generic;

namespace Contrive.Auth
{
  public interface IUserService
  {
    IUserServiceSettings Settings { get; }

    bool ChangePassword(string userName, string oldPassword, string newPassword);

    IUser GetUserByUserName(string userName);

    bool ValidateUser(string userName, string password);

    UserCreateStatus CreateUser(string userName, string password, string emailAddress, bool isApproved = true);

    bool DeleteAccount(string userName);

    IUser GetUserByEmailAddress(string emailAddress);

    string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow);

    bool ResetPasswordWithToken(string token, string newPassword);

    IUser GetUserFromPasswordResetToken(string token);

    IEnumerable<IUser> FindUsersForUserName(string searchTerm);

    IEnumerable<IUser> FindUsersForEmailAddress(string searchTerm);

    void UpdateUser(IUser user);
  }
}