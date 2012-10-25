using System;
using System.Linq;
using Contrive.Common;
using Contrive.Common.Extensions;

namespace Contrive.Auth
{
  /// <summary>
  ///   Duplicates the functionality of the WebSecurity class from ASP.NET WebPages.
  /// </summary>
  public class SecurityService : ISecurityService
  {
    public SecurityService(IUserServiceExtended userService,
                           IAuthenticationService authenticationService,
                           IPlatformAuthenticationService platformAuthenticationService,
                           IRoleService roleService)
    {
      _userService = userService;
      _authenticationService = authenticationService;
      _platformAuthenticationService = platformAuthenticationService;
      _roleService = roleService;
    }

    readonly IAuthenticationService _authenticationService;
    readonly IPlatformAuthenticationService _platformAuthenticationService;
    readonly IRoleService _roleService;
    readonly IUserServiceExtended _userService;

    public Guid CurrentUserId { get { return GetUserId(CurrentUserName); } }

    public string CurrentUserName { get { return _platformAuthenticationService.CurrentPrincipal.Identity.Name; } }

    public bool HasUserId { get { return !(CurrentUserId == Guid.Empty); } }

    public bool IsAuthenticated { get { return _platformAuthenticationService.UserIsAuthenticated; } }

    public bool Login(string userNameOrEmail, string password, bool rememberMe = false)
    {
      Verify.NotEmpty(userNameOrEmail, "userNameOrEmail");
      Verify.NotEmpty(password, "password");

      var user = _userService.GetUserByUserNameOrEmailAddress(userNameOrEmail);
      if (user.IsNull()) return false;
      return _authenticationService.LogOn(user.UserName, password, rememberMe);
    }

    public void Logout()
    {
      _authenticationService.LogOff();
    }

    public bool ChangePassword(string userName, string currentPassword, string newPassword)
    {
      Verify.NotEmpty(userName, "userName");
      Verify.NotEmpty(currentPassword, "currentPassword");
      Verify.NotEmpty(newPassword, "newPassword");

      var success = false;
      try
      {
        success = _userService.ChangePassword(userName, currentPassword, newPassword);
      }
      catch (ArgumentException) {}
      return success;
    }

    public bool ConfirmAccount(string accountConfirmationToken)
    {
      Verify.NotEmpty(accountConfirmationToken, "accountConfirmationToken");
      return _userService.ConfirmAccount(accountConfirmationToken);
    }

    public string CreateAccount(string userName,
                                string password,
                                string email,
                                bool requireConfirmationToken = false)
    {
      Verify.NotEmpty(userName, "userName");
      Verify.NotEmpty(password, "password");
      Verify.NotEmpty(email, "email");

      return _userService.CreateAccount(userName, password, email,
                                        requireConfirmationToken);
    }

    public string GeneratePasswordResetToken(string userName,
                                             int tokenExpirationInMinutesFromNow = 1440)
    {
      Verify.NotEmpty(userName, "userName");
      return _userService.GeneratePasswordResetToken(userName,
                                                     tokenExpirationInMinutesFromNow);
    }

    public bool UserExists(string userName)
    {
      Verify.NotEmpty(userName, "userName");
      return _userService.GetUserByUserName(userName) != null;
    }

    public Guid GetUserId(string userName)
    {
      Verify.NotEmpty(userName, "userName");
      var user = _userService.GetUserByUserName(userName);
      return user == null ? Guid.Empty : user.Id;
    }

    public Guid GetUserIdFromPasswordResetToken(string token)
    {
      Verify.NotEmpty(token, "token");
      return _userService.GetUserFromPasswordResetToken(token).Id;
    }

    public bool IsCurrentUser(string userName)
    {
      Verify.NotEmpty(userName, "userName");
      return string.Equals(CurrentUserName, userName, StringComparison.OrdinalIgnoreCase);
    }

    public bool IsConfirmed(string userName)
    {
      Verify.NotEmpty(userName, "userName");
      return _userService.IsConfirmed(userName);
    }

    public void RequireAuthenticatedUser()
    {
      var currentPrincipal = _platformAuthenticationService.CurrentPrincipal;
      if (currentPrincipal == null || !currentPrincipal.Identity.IsAuthenticated)
        _platformAuthenticationService.Deauthorize();
    }

    public void RequireUser(Guid userId)
    {
      Verify.NotEmpty(userId, "userId");
      if (!IsUserLoggedOn(userId)) _platformAuthenticationService.Deauthorize();
    }

    public void RequireUser(string userName)
    {
      Verify.NotEmpty(userName, "userName");
      if (!string.Equals(CurrentUserName, userName, StringComparison.OrdinalIgnoreCase))
        _platformAuthenticationService.Deauthorize();
    }

    public void RequireRoles(params string[] arrayOfRoles)
    {
      Verify.NotEmpty(arrayOfRoles, "arrayOfRoles");

      var isMissingRoles =
        arrayOfRoles.Any(role => !_roleService.IsUserInRole(CurrentUserName, role));

      if (!isMissingRoles) return;

      _platformAuthenticationService.Deauthorize();
    }

    public bool ResetPassword(string passwordResetToken, string newPassword)
    {
      Verify.NotEmpty(passwordResetToken, "passwordResetToken");
      Verify.NotEmpty(newPassword, "newPassword");

      return _userService.ResetPasswordWithToken(passwordResetToken, newPassword);
    }

    public bool IsAccountLockedOut(string userName,
                                   int allowedPasswordAttempts,
                                   int intervalInSeconds)
    {
      Verify.NotEmpty(userName, "userName");
      Verify.NotEmpty(allowedPasswordAttempts, "allowedPasswordAttempts");
      Verify.NotEmpty(intervalInSeconds, "intervalInSeconds");

      return IsAccountLockedOut(userName, allowedPasswordAttempts,
                                TimeSpan.FromSeconds(intervalInSeconds));
    }

    public bool IsAccountLockedOut(string userName,
                                   int allowedPasswordAttempts,
                                   TimeSpan interval)
    {
      Verify.NotEmpty(userName, "userName");
      Verify.NotEmpty(allowedPasswordAttempts, "allowedPasswordAttempts");
      Verify.NotEmpty(interval, "interval");

      var userManagementService = _userService;

      return (userManagementService.GetUserByUserName(userName) != null &&
              userManagementService.GetPasswordFailuresSinceLastSuccess(userName) >
              allowedPasswordAttempts &&
              userManagementService.GetLastPasswordFailureDate(userName).Add(interval) >
              DateTime.UtcNow);
    }

    public int GetPasswordFailuresSinceLastSuccess(string userName)
    {
      Verify.NotEmpty(userName, "userName");
      return _userService.GetPasswordFailuresSinceLastSuccess(userName);
    }

    public DateTime GetCreateDate(string userName)
    {
      Verify.NotEmpty(userName, "userName");
      return _userService.GetCreateDate(userName);
    }

    public DateTime GetPasswordChangedDate(string userName)
    {
      Verify.NotEmpty(userName, "userName");
      return _userService.GetPasswordChangedDate(userName);
    }

    public DateTime GetLastPasswordFailureDate(string userName)
    {
      Verify.NotEmpty(userName, "userName");
      return _userService.GetLastPasswordFailureDate(userName);
    }

    bool IsUserLoggedOn(Guid userId)
    {
      Verify.NotEmpty(userId, "userId");
      return CurrentUserId == userId;
    }
  }
}