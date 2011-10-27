using System;
using System.Web.Mvc;
using System.Web.Security;
using Contrive.Core;
using Contrive.Sample.Areas.Contrive.Models;

namespace Contrive.Sample.Areas.Contrive.Controllers
{
  public class ContriveAccountController : Controller
  {
    readonly IUserService _userService;
    readonly IAuthenticationService _authenticationService;
    readonly IEmailService _emailService;

    public ContriveAccountController(IUserService userService, IAuthenticationService authenticationService, IEmailService emailService)
    {
      _userService = userService;
      _authenticationService = authenticationService;
      _emailService = emailService;
    }

    public virtual ActionResult Index()
    {
      return View();
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
          else
            return RedirectToAction("Index", "Home");
        }
        else
        {
          var user = _userService.GetUser(model.UserName);
          if (user == null)
            ModelState.AddModelError("", "This account does not exist. Please try again.");
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
      var model = new RegisterViewModel();
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
          FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
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
      var viewModel = new ChangePasswordViewModel();

      return View(viewModel);
    }

    [Authorize]
    [HttpPost]
    public virtual ActionResult ChangePassword(ChangePasswordViewModel model)
    {
      if (ModelState.IsValid)
      {
        // ChangePassword will throw an exception rather
        // than return false in certain failure scenarios.
        bool changePasswordSucceeded;
        try
        {
          var currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
          changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
        }
        catch (Exception)
        {
          changePasswordSucceeded = false;
        }

        if (changePasswordSucceeded)
          return RedirectToAction("ChangePasswordSuccess");
        else
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
      var user = _userService.GetUserByEmail(model.EmailOrUserName);

      // Now reset the password
      string passwordResetToken = string.Empty;

      //if (_userService.RequiresQuestionAndAnswer)
      //  newPassword = user.ResetPassword(model.PasswordAnswer);
      //else
      passwordResetToken = _userService.GeneratePasswordResetToken(user.UserName);

      // Email the reset url to the user
      try
      {
        var settings = _userService.Settings;
        string body = _emailService.BuildMessageBody(user.UserName, passwordResetToken, settings.ContriveEmailTemplatePath);
        _emailService.SendEmail(settings.ContriveEmailFrom, user.Email, settings.ContriveEmailSubject, body);
      }
      catch (Exception) { }

      return RedirectToAction("ResetPasswordSuccess");
    }

    public ActionResult ResetPasswordSuccess()
    {
      return View();
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