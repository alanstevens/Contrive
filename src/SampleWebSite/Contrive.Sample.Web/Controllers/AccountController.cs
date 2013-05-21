using System;
using System.Web.Mvc;
using Contrive.Auth;
using Contrive.Sample.Web.Models;

namespace Contrive.Sample.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController(IUserService userService, ILogOnService logOnService)
        {
            _userService = userService;
            _logOnService = logOnService;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        readonly IUserService _userService;
        readonly ILogOnService _logOnService;

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && _logOnService.LogOn(model.UserName, model.Password, model.RememberMe))
                return RedirectToLocal(returnUrl);

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            _logOnService.LogOff();

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                var status = _userService.CreateUser(model.UserName, model.Password, model.EmailAddress);
                if (status == UserCreateStatus.Success)
                {
                    _logOnService.LogOn(model.UserName, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", UserCreateStatusToString(status));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess
                    ? "Your password has been changed."
                    : message == ManageMessageId.SetPasswordSuccess
                          ? "Your password has been set."
                          : message == ManageMessageId.RemoveLoginSuccess
                                ? "The external login was removed."
                                : "";
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(PasswordModel model)
        {
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (ModelState.IsValid)
            {
                // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    changePasswordSucceeded = _userService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (!changePasswordSucceeded) ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                else return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        static string UserCreateStatusToString(UserCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case UserCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case UserCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case UserCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case UserCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case UserCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case UserCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case UserCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case UserCreateStatus.ProviderError:
                    return
                        "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case UserCreateStatus.UserRejected:
                    return
                        "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return
                        "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }
}