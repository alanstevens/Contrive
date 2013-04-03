using System;
using System.Web.Mvc;
using System.Web.Security;
using Contrive.Auth.Membership;
using Contrive.Auth.Web.Mvc.Areas.Contrive.Models;
using Contrive.Common;
using Contrive.Common.Extensions;

namespace Contrive.Auth.Web.Mvc.Areas.Contrive.Controllers
{
    public class ContriveAccountController : Controller
    {
        public ContriveAccountController(IUserServiceExtended userService,
                                         ILogOnService logOnService,
                                         IEmailService emailService)
        {
            _userService = userService;
            _logOnService = logOnService;
            _emailService = emailService;
        }

        const int ONE_DAY_IN_MINUTES = 24*60;
        readonly ILogOnService _logOnService;
        readonly IEmailService _emailService;
        readonly IUserServiceExtended _userService;

        public virtual ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public virtual ActionResult LogOn()
        {
            var viewModel = new LogOnViewModel {EnablePasswordReset = _userService.Settings.EnablePasswordReset};
            return View(viewModel);
        }

        [HttpPost]
        public virtual ActionResult LogOn(LogOnViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (_logOnService.LogOn(model.UserName, model.Password, model.RememberMe))
                {
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") &&
                        !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\")) return Redirect(returnUrl);

                    return RedirectToAction("Index", "Home");
                }

                var user = _userService.GetUserByUserName(model.UserName);
                if (user == null) ModelState.AddModelError("", "The user name or password provided is incorrect.");
                else
                {
                    if (!user.IsApproved) ModelState.AddModelError("", "Your account has not been approved yet.");
                    else if (user.IsLockedOut) ModelState.AddModelError("", "Your account is currently locked.");
                    else ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return RedirectToAction("LogOn");
        }

        [HttpGet]
        public virtual ActionResult LogOff()
        {
            _logOnService.LogOff();

            return RedirectToAction("Index", "Home");
        }

        public virtual ActionResult Register()
        {
            var model = new RegisterViewModel {MinRequiredPasswordLength = _userService.Settings.MinRequiredPasswordLength};
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                //MembershipCreateStatus createStatus;
                // TODO: HAS 02/27/2013 Fix the namespacing for Membership here!
                //Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, out createStatus);

                ////if (createStatus == MembershipCreateStatus.Success)
                //{
                //  FormsAuthentication.SetAuthCookie(model.UserName, createPersistentCookie: false);
                //  return RedirectToAction("Index", "Home");
                //}
                //else ModelState.AddModelError("", ErrorCodeToString(createStatus));
            }

            return RedirectToAction("Register");
        }

        [Authorize]
        public virtual ActionResult ChangePassword()
        {
            var viewModel = new ChangePasswordViewModel
                            {
                                MinRequiredNonAlphanumericCharacters = _userService.Settings.MinRequiredNonAlphanumericCharacters,
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
                var changePasswordSucceeded = _userService.ChangePassword(User.Identity.Name, model.OldPassword,
                                                                          model.NewPassword);

                if (changePasswordSucceeded) return RedirectToAction("ChangePasswordSuccess");

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
            var user = _userService.GetUserByUserName(model.EmailOrUserName) ??
                       _userService.GetUserByEmailAddress(model.EmailOrUserName);

            if (user.IsNotNull())
            {
                // Now reset the password

                //if (_userService.RequiresQuestionAndAnswer)
                //  newPassword = user.ResetPassword(model.PasswordAnswer);
                //else
                var settings = _userService.Settings;
                // TODO: HAS 10/28/2011 Read duration from settings.
                var passwordResetToken = _userService.GeneratePasswordResetToken(user.UserName, ONE_DAY_IN_MINUTES);

                // Email the reset url to the user
                try
                {
                    var duration = user.PasswordVerificationTokenExpirationDate.GetValueOrDefault() - DateTime.UtcNow;
                    var rootUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                    var body = _emailService.BuildMessageBody(user.UserName, passwordResetToken, rootUrl,
                                                              Convert.ToInt32(duration.TotalHours),
                                                              settings.EmailTemplatePath);
                    _emailService.SendEmail(settings.EmailSender, user.Email, settings.EmailSubject, body);
                }
                catch (Exception) {}
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

            if (user.IsNull()) return RedirectToAction("Index", "Home");

            var model = new PasswordResetModel
                        {
                            User = user,
                            MinRequiredNonAlphanumericCharacters = _userService.Settings.MinRequiredNonAlphanumericCharacters,
                            MinRequiredPasswordLength = _userService.Settings.MinRequiredPasswordLength
                        };

            return View(model);
        }

        [HttpPost]
        public ActionResult PasswordReset(PasswordResetModel model)
        {
            if (ModelState.IsValid) _userService.ResetPasswordWithToken(model.User.PasswordVerificationToken, model.NewPassword);

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

        /// <summary>
        ///     This provides simple feedback to the modelstate in the case of errors.
        /// </summary>
        /// <param name="filterContext"> </param>
        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result is RedirectToRouteResult)
            {
                try
                {
                    // put the ModelState into TempData
                    TempData.Add("_MODELSTATE", ModelState);
                }
                catch (Exception)
                {
                    TempData.Clear();
                    // swallow exception
                }
            }
            else if (filterContext.Result is ViewResult && TempData.ContainsKey("_MODELSTATE"))
            {
                // merge modelstate from TempData
                var modelState = TempData["_MODELSTATE"] as ModelStateDictionary;
                foreach (var item in modelState) if (!ModelState.ContainsKey(item.Key)) ModelState.Add(item);
            }
            base.OnActionExecuted(filterContext);
        }
    }
}