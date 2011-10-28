using System;
using System.Web.Mvc;
using System.Web.Security;
using Contrive.Core;
using Contrive.Core.Extensions;
using Contrive.Sample.Areas.Contrive.Models;

namespace Contrive.Sample.Areas.Contrive.Controllers
{
  public class ContriveAccountController : Controller
  {
    public ContriveAccountController(IUserService userService, IAuthenticationService authenticationService,
                                     IEmailService emailService)
    {
      _userService = userService;
      _authenticationService = authenticationService;
      _emailService = emailService;
    }

    const int ONE_DAY_IN_MINUTES = 24 * 60;
    readonly IAuthenticationService _authenticationService;
    readonly IEmailService _emailService;
    readonly IUserService _userService;

    public virtual ActionResult Index()
    {
      return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public virtual ActionResult LogOn()
    {
      var viewModel = new LogOnViewModel
                      {
                        EnablePasswordReset = _userService.Settings.EnablePasswordReset
                      };
      return View(viewModel);
    }

    [HttpPost]
    public virtual ActionResult LogOn(LogOnViewModel model, string returnUrl)
    {
      if (ModelState.IsValid)
      {
        if (_authenticationService.LogOn(model.UserName, model.Password, model.RememberMe))
        {
          if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
              && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
            return Redirect(returnUrl);

          return RedirectToAction("Index", "Home");
        }

        var user = _userService.GetUser(model.UserName);
        if (user == null)
          ModelState.AddModelError("", "The user name or password provided is incorrect.");
        else
        {
          if (!user.IsApproved)
            ModelState.AddModelError("", "Your account has not been approved yet.");
          else if (user.IsLockedOut)
            ModelState.AddModelError("", "Your account is currently locked.");
          else
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
        }
      }

      // If we got this far, something failed, redisplay form
      return RedirectToAction("LogOn");
    }

    [HttpGet]
    public virtual ActionResult LogOff()
    {
      _authenticationService.LogOff();

      return RedirectToAction("Index", "Home");
    }

    public virtual ActionResult Register()
    {
      var model = new RegisterViewModel { MinRequiredPasswordLength = _userService.Settings.MinPasswordLength };
      return View(model);
    }

    [HttpPost]
    public virtual ActionResult Register(RegisterViewModel model)
    {
      if (ModelState.IsValid)
      {
        // Attempt to register the user
        MembershipCreateStatus createStatus;
        Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, out createStatus);

        if (createStatus == MembershipCreateStatus.Success)
        {
          FormsAuthentication.SetAuthCookie(model.UserName, createPersistentCookie: false);
          return RedirectToAction("Index", "Home");
        }
        else
          ModelState.AddModelError("", ErrorCodeToString(createStatus));
      }

      return RedirectToAction("Register");
    }

    [Authorize]
    public virtual ActionResult ChangePassword()
    {
      var viewModel = new ChangePasswordViewModel
                      {
                        MinRequiredNonAlphanumericCharacters =
                          _userService.Settings.MinRequiredNonAlphanumericCharacters,
                        MinRequiredPasswordLength = _userService.Settings.MinRequiredPasswordLength
                      };

      return View(viewModel);
    }

    [Authorize]
    [HttpPost]
    public virtual ActionResult ChangePassword(ChangePasswordViewModel model)
    {
      if (ModelState.IsValid)
      {
        bool changePasswordSucceeded = _userService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);

        if (changePasswordSucceeded)
          return RedirectToAction("ChangePasswordSuccess");

        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
      }

      // If we got this far, something failed, redisplay form
      return RedirectToAction("ChangePassword");
    }

    public virtual ActionResult ChangePasswordSuccess()
    {
      return View();
    }

    public ActionResult ResetPassword()
    {
      var viewModel = new ResetPasswordViewModel();
      //{
      //  RequiresQuestionAndAnswer = _userService.Settings.RequiresQuestionAndAnswer
      //};
      return View(viewModel);
    }

    [HttpPost]
    public ActionResult ResetPassword(ResetPasswordViewModel model)
    {
      // Get the userName by the email address
      var user = _userService.GetUser(model.EmailOrUserName) ??
                   _userService.GetUserByEmail(model.EmailOrUserName);

      if (user.IsNotNull())
      {
        // Now reset the password

        //if (_userService.RequiresQuestionAndAnswer)
        //  newPassword = user.ResetPassword(model.PasswordAnswer);
        //else
        var settings = _userService.Settings;
        // TODO: HAS 10/28/2011 Read duration from settings.
        string passwordResetToken = _userService.GeneratePasswordResetToken(user.UserName, ONE_DAY_IN_MINUTES);

        // Email the reset url to the user
        try
        {
          var duration = user.PasswordVerificationTokenExpirationDate.GetValueOrDefault() - DateTime.UtcNow;
          string rootUrl = Request.Url.GetLeftPart(UriPartial.Authority);
          string body = _emailService.BuildMessageBody(user.UserName, passwordResetToken, rootUrl, Convert.ToInt32(duration.TotalHours), settings.ContriveEmailTemplatePath);
          _emailService.SendEmail(settings.ContriveEmailFrom, user.Email, settings.ContriveEmailSubject, body);
        }
        catch (Exception) { }
      }

      return RedirectToAction("ResetPasswordSuccess");
    }

    public ActionResult ResetPasswordSuccess()
    {
      return View();
    }

    public ActionResult PasswordReset(string token)
    {
      var user = _userService.GetUserFromPasswordResetToken(token);

      if (user.IsNull())
        return RedirectToAction("Index", "Home");

      var model = new PasswordResetModel
      {
        User = user,
        MinRequiredNonAlphanumericCharacters =
          _userService.Settings.MinRequiredNonAlphanumericCharacters,
        MinRequiredPasswordLength = _userService.Settings.MinRequiredPasswordLength
      };

      return View(model);
    }

    [HttpPost]
    public ActionResult PasswordReset(PasswordResetModel model)
    {
      if (ModelState.IsValid)
      {
        _userService.ResetPasswordWithToken(model.User.PasswordVerificationToken, model.NewPassword);
      }

      return RedirectToAction("Index", "Home");
    }

    static string ErrorCodeToString(MembershipCreateStatus createStatus)
    {
      // See http://go.microsoft.com/fwlink/?LinkID=177550 for
      // a full list of status codes.
      switch (createStatus)
      {
        case MembershipCreateStatus.DuplicateUserName:
          return "User name already exists. Please enter a different user name.";

        case MembershipCreateStatus.DuplicateEmail:
          return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

        case MembershipCreateStatus.InvalidPassword:
          return "The password provided is invalid. Please enter a valid password value.";

        case MembershipCreateStatus.InvalidEmail:
          return "The e-mail address provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidAnswer:
          return "The password retrieval answer provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidQuestion:
          return "The password retrieval question provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidUserName:
          return "The user name provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.ProviderError:
          return
            "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        case MembershipCreateStatus.UserRejected:
          return
            "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        default:
          return
            "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
      }
    }
  }
}