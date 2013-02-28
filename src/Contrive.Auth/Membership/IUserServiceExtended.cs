using System;
using System.Collections.Generic;

namespace Contrive.Auth.Membership
{
    public interface IUserServiceExtended
    {
        IUserServiceSettings Settings { get; }

        bool ChangePassword(string userName, string oldPassword, string newPassword);

        IUserExtended GetUserByUserName(string userName);

        bool ValidateUser(string userName, string password);

        UserCreateStatus CreateUser(string userName, string password, string emailAddress, bool isApproved = true);

        bool DeleteAccount(string userName);

        IUserExtended GetUserByEmailAddress(string emailAddress);

        void UpdateUser(IUserExtended user);

        bool VerifyUser(IUserExtended user, string password);

        string CreateAccount(string userName, string password, string emailAddress, bool requireConfirmationToken = false);

        bool ConfirmAccount(string token);

        bool IsConfirmed(string userName);

        string ValidateUserExtended(string userNameOrEmailAddress, string password);

        DateTime GetPasswordChangedDate(string userName);

        DateTime GetCreateDate(string userName);

        int GetPasswordFailuresSinceLastSuccess(string userName);

        DateTime GetLastPasswordFailureDate(string userName);

        IUserExtended GetUserByUserNameOrEmailAddress(string userNameOrEmailAddress);

        string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow);

        bool ResetPasswordWithToken(string token, string newPassword);

        IUserExtended GetUserFromPasswordResetToken(string token);

        IEnumerable<IUserExtended> FindUsersForUserName(string searchTerm);

        IEnumerable<IUserExtended> FindUsersForEmailAddress(string searchTerm);
    }
}