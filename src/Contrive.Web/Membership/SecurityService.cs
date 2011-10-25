using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Security;
using Contrive.Core;

namespace Contrive.Web.Membership
{
  /// <summary>
  ///   Duplicates the functionality of the WebSecurity class from ASP.NET WebPages.
  /// </summary>
  public class SecurityService : ISecurityService
  {
    readonly IUserService _userService;

    public SecurityService(IUserService userService)
    {
      _userService = userService;
    }

    public Guid CurrentUserId
    {
      get { return GetUserId(CurrentUserName); }
    }

    public string CurrentUserName
    {
      get { return Context.User.Identity.Name; }
    }

    public bool HasUserId
    {
      get { return !(CurrentUserId == Guid.Empty); }
    }

    public bool IsAuthenticated
    {
      get { return Request.IsAuthenticated; }
    }

    HttpContextBase Context
    {
      get { return new HttpContextWrapper(HttpContext.Current); }
    }

    HttpRequestBase Request
    {
      get { return Context.Request; }
    }

    HttpResponseBase Response
    {
      get { return Context.Response; }
    }

    public bool Login(string userNameOrEmail, string password, bool persistCookie = false)
    {
      var success = _userService.ValidateUserExtended(userNameOrEmail, password);

      if (!(string.IsNullOrEmpty(success)))
      {
        FormsAuthentication.SetAuthCookie(success, persistCookie);
        return true;
      }

      return false;
    }

    public void Logout()
    {
      FormsAuthentication.SignOut();
    }

    public bool ChangePassword(string userName, string currentPassword, string newPassword)
    {
      bool success = false;
      try
      {
        success = _userService.ChangePassword(userName, currentPassword, newPassword);
      }
      catch (ArgumentException) { }
      return success;
    }

    public bool ConfirmAccount(string accountConfirmationToken)
    {
      return _userService.ConfirmAccount(accountConfirmationToken);
    }

    public string CreateAccount(string userName, string password, string email,
                                       bool requireConfirmationToken = false)
    {
      return _userService.CreateAccount(userName, password, email, requireConfirmationToken);
    }

    public string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow = 1440)
    {
      return _userService.GeneratePasswordResetToken(userName, tokenExpirationInMinutesFromNow);
    }

    public bool UserExists(string userName)
    {
      return _userService.GetUser(userName) != null;
    }

    public Guid GetUserId(string userName)
    {
      var user = _userService.GetUser(userName);
      return user == null ? Guid.Empty : user.Id;
    }

    public Guid GetUserIdFromPasswordResetToken(string token)
    {
      return _userService.GetUserIdFromPasswordResetToken(token);
    }

    public bool IsCurrentUser(string userName)
    {
      return string.Equals(CurrentUserName, userName, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsConfirmed(string userName)
    {
      return _userService.IsConfirmed(userName);
    }

    bool IsUserLoggedOn(Guid userId)
    {
      return CurrentUserId == userId;
    }

    public void RequireAuthenticatedUser()
    {
      var user = Context.User;
      if (user == null || !user.Identity.IsAuthenticated)
        Response.SetStatus(HttpStatusCode.Unauthorized);
    }

    public void RequireUser(Guid userId)
    {
      if (!IsUserLoggedOn(userId))
        Response.SetStatus(HttpStatusCode.Unauthorized);
    }

    public void RequireUser(string userName)
    {
      if (!string.Equals(CurrentUserName, userName, StringComparison.OrdinalIgnoreCase))
        Response.SetStatus(HttpStatusCode.Unauthorized);
    }

    public void RequireRoles(params string[] arrayOfRoles)
    {
      var isMissingRoles = arrayOfRoles.Any(role => !Roles.IsUserInRole(CurrentUserName, role));

      if (!isMissingRoles) return;

      Response.SetStatus(HttpStatusCode.Unauthorized);
    }

    public bool ResetPassword(string passwordResetToken, string newPassword)
    {
      return _userService.ResetPasswordWithToken(passwordResetToken, newPassword);
    }

    public bool IsAccountLockedOut(string userName, int allowedPasswordAttempts, int intervalInSeconds)
    {
      return IsAccountLockedOut(userName, allowedPasswordAttempts, TimeSpan.FromSeconds(intervalInSeconds));
    }

    public bool IsAccountLockedOut(string userName, int allowedPasswordAttempts, TimeSpan interval)
    {
      var userManagementService = _userService;

      return (userManagementService.GetUser(userName) != null &&
              userManagementService.GetPasswordFailuresSinceLastSuccess(userName) > allowedPasswordAttempts &&
              userManagementService.GetLastPasswordFailureDate(userName).Add(interval) > DateTime.UtcNow);
    }

    public int GetPasswordFailuresSinceLastSuccess(string userName)
    {
      return _userService.GetPasswordFailuresSinceLastSuccess(userName);
    }

    public DateTime GetCreateDate(string userName)
    {
      return _userService.GetCreateDate(userName);
    }

    public DateTime GetPasswordChangedDate(string userName)
    {
      return _userService.GetPasswordChangedDate(userName);
    }

    public DateTime GetLastPasswordFailureDate(string userName)
    {
      return _userService.GetLastPasswordFailureDate(userName);
    }
  }

  public static class HttpContextBaseExtensions
  {
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