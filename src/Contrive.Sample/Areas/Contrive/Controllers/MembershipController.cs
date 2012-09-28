using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Contrive.Core;
using Contrive.Sample.Areas.Contrive.Models;

namespace Contrive.Sample.Areas.Contrive.Controllers
{
  [Authorize(Roles = "Admin")]
  public class MembershipController : Controller
  {
    public MembershipController(IRoleService roleService, IUserService userService)
    {
      _roleService = roleService;
      _userService = userService;
    }

    readonly IRoleService _roleService;
    readonly IUserService _userService;

    public virtual ActionResult Index(string searchterm, string filterby)
    {
      var viewModel = new ManageUsersViewModel {Users = null};

      if (!string.IsNullOrEmpty(searchterm))
      {
        if (filterby == "email") viewModel.Users = _userService.FindUsersForEmail(searchterm);
        else if (filterby == "username") viewModel.Users = _userService.FindUsersForUserName(searchterm);
      }

      return View(viewModel);
    }

    public virtual ActionResult CreateUser()
    {
      var model = new RegisterViewModel {MinRequiredPasswordLength = _userService.Settings.MinRequiredPasswordLength};
      //{
      //  RequireSecretQuestionAndAnswer = _userService.RequiresQuestionAndAnswer
      //};
      return View(model);
    }

    [HttpPost]
    public virtual ActionResult CreateUser(RegisterViewModel model)
    {
      var status = _userService.CreateUser(model.UserName, model.Password, model.Email, model.Approve);

      if (status == UserCreateStatus.Success)
      {
        var user = _userService.GetUserByUserName(model.UserName);

        return new RedirectToRouteResult(new RouteValueDictionary(new {action = "GrantRolesToUser", controller = "Membership", username = user.UserName}));
      }
      // TODO: HAS 09/27/2012 Report create failure
      return RedirectToAction("Index");
    }

    [HttpGet]
    public ActionResult CheckForUniqueUser(string userName)
    {
      var user = _userService.GetUserByUserName(userName);
      var response = new JsonResponse {Exists = (user != null)};

      return Json(response, JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    //[MultiButtonFormSubmit(ActionName = "UpdateDeleteCancel", SubmitButton = "DeleteUser")]
    public ActionResult DeleteUser(string userName)
    {
      if (string.IsNullOrEmpty(userName)) throw new ArgumentNullException("userName");

      try
      {
        _userService.DeleteAccount(userName);
        return RedirectToAction("Index");
      }
      catch (Exception ex)
      {
        TempData["ErrorMessage"] = "There was an error deleting this user. - " + ex.Message;
      }

      return RedirectToAction("Update", new {userName});
    }

    [HttpGet]
    public ActionResult Update(string userName)
    {
      var user = _userService.GetUserByUserName(userName);

      var viewModel = new UserViewModel {User = user, Roles = _roleService.GetRolesForUser(userName)};
      //viewModel.RequiresSecretQuestionAndAnswer = _userService.RequiresQuestionAndAnswer;

      return View(viewModel);
    }

    [HttpPost]
    //[ActionName("Update")]
    //[MultiButtonFormSubmit(ActionName = "UpdateDeleteCancel", SubmitButton = "UpdateUser")]
    public ActionResult UpdateUser(string UserName)
    {
      Verify.NotEmpty(UserName, "userName");

      var user = _userService.GetUserByUserName(UserName);

      try
      {
        //user.Comment = Request["User.Comment"];
        user.Email = Request["User.Email"];

        _userService.UpdateUser(user);

        TempData["SuccessMessage"] = "The user was updated successfully!";
      }
      catch (Exception)
      {
        TempData["ErrorMessage"] = "There was an error updating this user.";
      }

      return RedirectToAction("Update", new {userName = user.UserName});
    }

    [HttpPost]
    public ActionResult Unlock(string userName)
    {
      var response = new JsonResponse();

      var user = _userService.GetUserByUserName(userName);

      try
      {
        //user.UnlockUser();
        response.Success = true;
        response.Message = "User unlocked successfully!";
        response.Locked = false;
        response.LockedStatus = (response.Locked) ? "Locked" : "Unlocked";
      }
      catch (Exception)
      {
        response.Success = false;
        response.Message = "User unlocked failed.";
      }

      return Json(response);
    }

    [HttpPost]
    public ActionResult ApproveDeny(string userName)
    {
      var response = new JsonResponse();

      var user = _userService.GetUserByUserName(userName);

      try
      {
        user.IsApproved = !user.IsApproved;
        _userService.UpdateUser(user);

        var approvedMsg = (user.IsApproved) ? "Approved" : "Denied";

        response.Success = true;
        response.Message = "User " + approvedMsg + " successfully!";
        response.Approved = user.IsApproved;
        response.ApprovedStatus = (user.IsApproved) ? "Approved" : "Not approved";
      }
      catch (Exception)
      {
        response.Success = false;
        response.Message = "User unlocked failed.";
      }

      return Json(response);
    }

    [HttpPost]
    //[MultiButtonFormSubmit(ActionName = "UpdateDeleteCancel", SubmitButton = "UserCancel")]
    public ActionResult Cancel()
    {
      return RedirectToAction("Index");
    }

    public virtual ActionResult GrantRolesToUser(string username)
    {
      if (string.IsNullOrEmpty(username)) return RedirectToAction("Index");

      var model = new GrantRolesToUserViewModel();
      model.UserName = username;
      var allRoles = _roleService.GetAllRoles();
      model.AvailableRoles = (string.IsNullOrEmpty(username) ? new SelectList(allRoles) : new SelectList(allRoles.Where(r => !_roleService.GetRolesForUser(username).Contains(r))));
      model.GrantedRoles = (string.IsNullOrEmpty(username) ? new SelectList(new string[] {}) : new SelectList(_roleService.GetRolesForUser(username)));

      return View(model);
    }

    [HttpPost]
    public virtual ActionResult GrantRolesToUser(string userName, string roles)
    {
      var response = new JsonResponse();

      if (string.IsNullOrEmpty(userName))
      {
        response.Success = false;
        response.Message = "The userName is missing.";
        return Json(response);
      }

      var roleNames = roles.Substring(0, roles.Length - 1).Split(',');

      if (roleNames.Length == 0)
      {
        response.Success = false;
        response.Message = "No roles have been granted to the user.";
        return Json(response);
      }

      try
      {
        _roleService.AddUsersToRoles(new[] {userName}, roleNames);

        response.Success = true;
        response.Message = "The Role(s) has been GRANTED successfully to " + userName;
      }
      catch (Exception)
      {
        response.Success = false;
        response.Message = "There was a problem adding the user to the roles.";
      }

      return Json(response);
    }

    [HttpPost]
    public ActionResult RevokeRolesForUser(string userName, string roles)
    {
      var response = new JsonResponse();

      if (string.IsNullOrEmpty(userName))
      {
        response.Success = false;
        response.Message = "The userName is missing.";
        return Json(response);
      }

      if (string.IsNullOrEmpty(roles))
      {
        response.Success = false;
        response.Message = "Roles is missing";
        return Json(response);
      }

      var roleNames = roles.Substring(0, roles.Length - 1).Split(',');

      if (roleNames.Length == 0)
      {
        response.Success = false;
        response.Message = "No roles are selected to be revoked.";
        return Json(response);
      }

      try
      {
        _roleService.RemoveUsersFromRoles(new[] {userName}, roleNames);

        response.Success = true;
        response.Message = "The Role(s) has been REVOKED successfully for " + userName;
      }
      catch (Exception)
      {
        response.Success = false;
        response.Message = "There was a problem revoking roles for the user.";
      }

      return Json(response);
    }
  }
}