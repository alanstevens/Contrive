using System;

namespace Contrive.Core
{
  public interface IUserService
  {
    string ApplicationName { get; set; }
    int MinPasswordLength { get; }
    bool EnablePasswordRetrieval { get; }
    bool EnablePasswordReset { get; }
    bool RequiresQuestionAndAnswer { get; }
    int MaxInvalidPasswordAttempts { get; }
    int PasswordAttemptWindow { get; }
    bool RequiresUniqueEmail { get; }
    UserPasswordFormat PasswordFormat { get; }
    int MinRequiredPasswordLength { get; }
    int MinRequiredNonAlphanumericCharacters { get; }
    string PasswordStrengthRegularExpression { get; }

    bool ChangePassword(string userName, string oldPassword, string newPassword);

    IUser GetUser(string userName);

    bool ValidateUser(string userName, string password);

    UserCreateStatus CreateUser(string userName,
                                string password,
                                string email,
                                string passwordQuestion,
                                string passwordAnswer,
                                bool isApproved,
                                object providerUserKey);

    bool VerifyUser(IUser user, string password);

    UserCreateStatus CreateUser(string userName, string password, string email);

    string GeneratePasswordResetToken(string userName);

    string CreateAccount(string userName, string password, string email, bool requireConfirmationToken = false);

    bool ConfirmAccount(string accountConfirmationToken);

    bool DeleteAccount(string userName);

    bool IsConfirmed(string userName);

    string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow);

    bool ResetPasswordWithToken(string token, string newPassword);

    string ValidateUserExtended(string userNameOrEmail, string password);

    DateTime GetPasswordChangedDate(string userName);

    DateTime GetCreateDate(string userName);

    int GetPasswordFailuresSinceLastSuccess(string userName);

    Guid GetUserIdFromPasswordResetToken(string token);

    DateTime GetLastPasswordFailureDate(string userName);

    IUser GetUserByEmail(string emailAddress);
  }
}