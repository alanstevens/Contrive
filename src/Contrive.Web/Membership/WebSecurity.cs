using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Security;
using Contrive.Core;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Web.Membership
{
  /// <summary>
  ///   Duplicates the functionality of the WebSecurity class from ASP.NET WebPages.
  /// </summary>
  public static class WebSecurity
  {
    public static Guid CurrentUserId
    {
      get { return GetUserId(CurrentUserName); }
    }

    public static string CurrentUserName
    {
      get { return Context.User.Identity.Name; }
    }

    public static bool HasUserId
    {
      get { return !(CurrentUserId == Guid.Empty); }
    }

    public static bool IsAuthenticated
    {
      get { return Request.IsAuthenticated; }
    }

    static HttpContextBase Context
    {
      get { return new HttpContextWrapper(HttpContext.Current); }
    }

    static HttpRequestBase Request
    {
      get { return Context.Request; }
    }

    static HttpResponseBase Response
    {
      get { return Context.Response; }
    }

    static IUserProvider GetUserManagementService()
    {
      return ServiceLocator.Current.GetInstance<IUserProvider>();
    }

    public static bool Login(string userNameOrEmail, string password, bool persistCookie = false)
    {
      var success = GetUserManagementService().ValidateUserExtended(userNameOrEmail, password);
      if (!(string.IsNullOrEmpty(success)))
      {
        FormsAuthentication.SetAuthCookie(success, persistCookie);
        return true;
      }

      return false;
    }

    public static void Logout()
    {
      FormsAuthentication.SignOut();
    }

    public static bool ChangePassword(string userName, string currentPassword, string newPassword)
    {
      bool success = false;
      try
      {
        success = GetUserManagementService().ChangePassword(userName, currentPassword, newPassword);
      }
      catch (ArgumentException) { }
      return success;
    }

    public static bool ConfirmAccount(string accountConfirmationToken)
    {
      return GetUserManagementService().ConfirmAccount(accountConfirmationToken);
    }

    public static string CreateAccount(string userName, string password, string email,
                                       bool requireConfirmationToken = false)
    {
      return GetUserManagementService().CreateAccount(userName, password, email, requireConfirmationToken);
    }

    public static string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow = 1440)
    {
      return GetUserManagementService().GeneratePasswordResetToken(userName, tokenExpirationInMinutesFromNow);
    }

    public static bool UserExists(string userName)
    {
      return GetUserManagementService().GetUser(userName) != null;
    }

    public static Guid GetUserId(string userName)
    {
      var user = GetUserManagementService().GetUser(userName);
      return user == null ? Guid.Empty : user.Id;
    }

    public static Guid GetUserIdFromPasswordResetToken(string token)
    {
      return GetUserManagementService().GetUserIdFromPasswordResetToken(token);
    }

    public static bool IsCurrentUser(string userName)
    {
      return string.Equals(CurrentUserName, userName, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsConfirmed(string userName)
    {
      return GetUserManagementService().IsConfirmed(userName);
    }

    static bool IsUserLoggedOn(Guid userId)
    {
      return CurrentUserId == userId;
    }

    public static void RequireAuthenticatedUser()
    {
      var user = Context.User;
      if (user == null || !user.Identity.IsAuthenticated)
        SetStatus(Response, HttpStatusCode.Unauthorized);
    }

    public static void RequireUser(Guid userId)
    {
      if (!IsUserLoggedOn(userId))
        SetStatus(Response, HttpStatusCode.Unauthorized);
    }

    public static void RequireUser(string userName)
    {
      if (!string.Equals(CurrentUserName, userName, StringComparison.OrdinalIgnoreCase))
        SetStatus(Response, HttpStatusCode.Unauthorized);
    }

    public static void RequireRoles(params string[] arrayOfRoles)
    {
      var isMissingRoles = arrayOfRoles.Any(role => !Roles.IsUserInRole(CurrentUserName, role));

      if (!isMissingRoles) return;

      SetStatus(Response, HttpStatusCode.Unauthorized);
    }

    public static bool ResetPassword(string passwordResetToken, string newPassword)
    {
      return GetUserManagementService().ResetPasswordWithToken(passwordResetToken, newPassword);
    }

    public static bool IsAccountLockedOut(string userName, int allowedPasswordAttempts, int intervalInSeconds)
    {
      return IsAccountLockedOut(userName, allowedPasswordAttempts, TimeSpan.FromSeconds(intervalInSeconds));
    }

    public static bool IsAccountLockedOut(string userName, int allowedPasswordAttempts, TimeSpan interval)
    {
      var userManagementService = GetUserManagementService();

      return (userManagementService.GetUser(userName) != null &&
              userManagementService.GetPasswordFailuresSinceLastSuccess(userName) > allowedPasswordAttempts &&
              userManagementService.GetLastPasswordFailureDate(userName).Add(interval) > DateTime.UtcNow);
    }

    public static int GetPasswordFailuresSinceLastSuccess(string userName)
    {
      return GetUserManagementService().GetPasswordFailuresSinceLastSuccess(userName);
    }

    public static DateTime GetCreateDate(string userName)
    {
      return GetUserManagementService().GetCreateDate(userName);
    }

    public static DateTime GetPasswordChangedDate(string userName)
    {
      return GetUserManagementService().GetPasswordChangedDate(userName);
    }

    public static DateTime GetLastPasswordFailureDate(string userName)
    {
      return GetUserManagementService().GetLastPasswordFailureDate(userName);
    }

    public static void SetStatus(this HttpResponseBase response, HttpStatusCode httpStatusCode)
    {
      SetStatus(response, (int)httpStatusCode);
    }

    public static void SetStatus(this HttpResponseBase response, int httpStatusCode)
    {
      response.StatusCode = httpStatusCode;
      response.End();
    }
  }
}