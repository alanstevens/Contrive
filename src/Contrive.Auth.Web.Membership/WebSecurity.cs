using System;
using Contrive.Auth.Membership;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Auth.Web.Membership
{
    /// <summary>
    ///     Duplicates the functionality of the WebSecurity class from ASP.NET WebPages.
    /// </summary>
    public static class WebSecurity
    {
        public static Guid CurrentUserId { get { return GetUserId(CurrentUserName); } }

        public static string CurrentUserName { get { return GetSecurityService().CurrentUserName; } }

        public static bool HasUserId { get { return !(CurrentUserId == Guid.Empty); } }

        public static bool IsAuthenticated { get { return GetSecurityService().IsAuthenticated; } }

        static ISecurityService GetSecurityService()
        {
            return ServiceLocator.Current.GetInstance<ISecurityService>();
        }

        public static bool Login(string userNameOrEmail, string password, bool rememberMe = false)
        {
            return GetSecurityService().Login(userNameOrEmail, password, rememberMe);
        }

        public static void Logout()
        {
            GetSecurityService().Logout();
        }

        public static bool ChangePassword(string userName, string currentPassword, string newPassword)
        {
            return GetSecurityService().ChangePassword(userName, currentPassword, newPassword);
        }

        public static bool ConfirmAccount(string accountConfirmationToken)
        {
            return GetSecurityService().ConfirmAccount(accountConfirmationToken);
        }

        public static string CreateAccount(string userName,
                                           string password,
                                           string email,
                                           bool requireConfirmationToken = false)
        {
            return GetSecurityService().CreateAccount(userName, password, email, requireConfirmationToken);
        }

        public static string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow = 1440)
        {
            return GetSecurityService().GeneratePasswordResetToken(userName, tokenExpirationInMinutesFromNow);
        }

        public static bool UserExists(string userName)
        {
            return GetSecurityService().UserExists(userName);
        }

        public static Guid GetUserId(string userName)
        {
            return GetSecurityService().GetUserId(userName);
        }

        public static Guid GetUserIdFromPasswordResetToken(string token)
        {
            return GetSecurityService().GetUserIdFromPasswordResetToken(token);
        }

        public static bool IsCurrentUser(string userName)
        {
            return string.Equals(CurrentUserName, userName, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsConfirmed(string userName)
        {
            return GetSecurityService().IsConfirmed(userName);
        }

        public static void RequireAuthenticatedUser()
        {
            GetSecurityService().RequireAuthenticatedUser();
        }

        public static void RequireUser(Guid userId)
        {
            GetSecurityService().RequireUser(userId);
        }

        public static void RequireUser(string userName)
        {
            GetSecurityService().RequireUser(userName);
        }

        public static void RequireRoles(params string[] arrayOfRoles)
        {
            GetSecurityService().RequireRoles(arrayOfRoles);
        }

        public static bool ResetPassword(string passwordResetToken, string newPassword)
        {
            return GetSecurityService().ResetPassword(passwordResetToken, newPassword);
        }

        public static bool IsAccountLockedOut(string userName, int allowedPasswordAttempts, int intervalInSeconds)
        {
            return IsAccountLockedOut(userName, allowedPasswordAttempts, TimeSpan.FromSeconds(intervalInSeconds));
        }

        public static bool IsAccountLockedOut(string userName, int allowedPasswordAttempts, TimeSpan interval)
        {
            return GetSecurityService().IsAccountLockedOut(userName, allowedPasswordAttempts, interval);
        }

        public static int GetPasswordFailuresSinceLastSuccess(string userName)
        {
            return GetSecurityService().GetPasswordFailuresSinceLastSuccess(userName);
        }

        public static DateTime GetCreateDate(string userName)
        {
            return GetSecurityService().GetCreateDate(userName);
        }

        public static DateTime GetPasswordChangedDate(string userName)
        {
            return GetSecurityService().GetPasswordChangedDate(userName);
        }

        public static DateTime GetLastPasswordFailureDate(string userName)
        {
            return GetSecurityService().GetLastPasswordFailureDate(userName);
        }
    }
}