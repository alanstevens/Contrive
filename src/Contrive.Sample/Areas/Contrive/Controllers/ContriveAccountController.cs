using System;
using System.IO;
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

    public ContriveAccountController(IUserService userService, IAuthenticationService authenticationService)
    {
      _userService = userService;
      _authenticationService = authenticationService;
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
                        EnablePasswordReset = _userService.EnablePasswordReset
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
      var model = new RegisterViewModel
                  {
                    RequireSecretQuestionAndAnswer = _userService.RequiresQuestionAndAnswer
                  };
      return View(model);
    }

    [HttpPost]
    public virtual ActionResult Register(RegisterViewModel model)
    {
      if (ModelState.IsValid)
      {
        // Attempt to register the user
        MembershipCreateStatus createStatus;
        Membership.CreateUser(model.UserName, model.Password, model.Email, model.SecretQuestion, model.SecretAnswer,
                              true, out createStatus);

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

    public ActionResult ForgotPassword()
    {
      var viewModel = new ForgotPasswordViewModel
                      {
                        RequiresQuestionAndAnswer = _userService.RequiresQuestionAndAnswer
                      };
      return View(viewModel);
    }

    [HttpPost]
    public ActionResult ForgotPassword(ForgotPasswordViewModel model)
    {
      // Get the userName by the email address
      string userName = _userService.GetUserNameByEmail(model.Email);

      // Get the user by the userName
      var user = _userService.GetUser(userName);

      // Now reset the password
      string newPassword = string.Empty;

      if (_userService.RequiresQuestionAndAnswer)
        newPassword = user.ResetPassword(model.PasswordAnswer);
      else
        newPassword = user.ResetPassword();

      // Email the new pasword to the user
      try
      {
        string body = BuildMessageBody(user.UserName, newPassword, ConfigSettings.SecurityGuardEmailTemplatePath);
        Mail(model.Email, ConfigSettings.SecurityGuardEmailFrom, ConfigSettings.SecurityGuardEmailSubject, body, true);
      }
      catch (Exception) { }

      return RedirectToAction("ForgotPasswordSuccess");
    }

    public ActionResult ForgotPasswordSuccess()
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

    void Mail(string emailTo, string emailFrom, string subject, string body, bool isHtml)
    {
      Email email = new Email();
      email.ToList = emailTo;
      email.FromEmail = emailFrom;
      email.Subject = subject;
      email.MessageBody = body;
      email.isHTML = isHtml;

      email.SendEmail(email);
    }

    string BuildMessageBody(string userName, string password, string filePath)
    {
      string body = string.Empty;

      FileInfo fi = new FileInfo(Server.MapPath(filePath));
      string text = string.Empty;

      if (fi.Exists)
      {
        using (StreamReader sr = fi.OpenText())
        {
          text = sr.ReadToEnd();
        }
        text = text.Replace("%UserName%", userName);
        text = text.Replace("%Password%", password);
      }
      body = text;

      return body;
    }
  }
}