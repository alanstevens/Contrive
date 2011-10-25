using System;

namespace Contrive.Core
{
  public interface ISecurityService
  {
    Guid CurrentUserId { get; }
    string CurrentUserName { get; }
    bool HasUserId { get; }
    bool IsAuthenticated { get; }

    bool Login(string userNameOrEmail, string password, bool persistCookie = false);

    void Logout();

    bool ChangePassword(string userName, string currentPassword, string newPassword);

    bool ConfirmAccount(string accountConfirmationToken);

    string CreateAccount(string userName, string password, string email,
                         bool requireConfirmationToken = false);

    string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow = 1440);

    bool UserExists(string userName);

    Guid GetUserId(string userName);

    Guid GetUserIdFromPasswordResetToken(string token);

    bool IsCurrentUser(string userName);

    bool IsConfirmed(string userName);

    void RequireAuthenticatedUser();

    void RequireUser(Guid userId);

    void RequireUser(string userName);

    void RequireRoles(params string[] arrayOfRoles);

    bool ResetPassword(string passwordResetToken, string newPassword);

    bool IsAccountLockedOut(string userName, int allowedPasswordAttempts, int intervalInSeconds);

    bool IsAccountLockedOut(string userName, int allowedPasswordAttempts, TimeSpan interval);

    int GetPasswordFailuresSinceLastSuccess(string userName);

    DateTime GetCreateDate(string userName);

    DateTime GetPasswordChangedDate(string userName);

    DateTime GetLastPasswordFailureDate(string userName);
  }
}