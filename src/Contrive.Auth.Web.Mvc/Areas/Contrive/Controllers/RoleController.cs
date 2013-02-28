using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Contrive.Auth.Membership;
using Contrive.Auth.Web.Mvc.Areas.Contrive.Models;

namespace Contrive.Auth.Web.Mvc.Areas.Contrive.Controllers
{
  [Authorize(Roles = "Admin")]
  public class RoleController : Controller
  {
    public RoleController(IRoleService roleService)
    {
      _roleService = roleService;
    }

    readonly IRoleService _roleService;

    public virtual ActionResult Index()
    {
      var model = new ManageRolesViewModel
      {Roles = new SelectList(_roleService.GetAllRoles()), RoleList = _roleService.GetAllRoles()};

      return View(model);
    }

    //[HttpGet]
    //public virtual ActionResult CreateRole()
    //{
    //  return View(new RoleViewModel());
    //}

    [HttpPost]
    public virtual ActionResult CreateRole(string roleName)
    {
      var response = new JsonResponse();

      if (string.IsNullOrEmpty(roleName))
      {
        response.Success = false;
        response.Message = "You must enter a role name.";
        response.CssClass = "red";

        return Json(response);
      }

      try
      {
        _roleService.CreateRole(roleName);

        if (Request.IsAjaxRequest())
        {
          response.Success = true;
          response.Message = "Role created successfully!";
          response.CssClass = "green";

          return Json(response);
        }

        return RedirectToAction("Index");
      }
      catch (Exception ex)
      {
        if (Request.IsAjaxRequest())
        {
          response.Success = false;
          response.Message = ex.Message;
          response.CssClass = "red";

          return Json(response);
        }

        ModelState.AddModelError("", ex.Message);
      }

      return RedirectToAction("Index");
    }

    [HttpPost]
    public virtual ActionResult DeleteRole(string roleName)
    {
      var response = new JsonResponse();

      if (string.IsNullOrEmpty(roleName))
      {
        response.Success = false;
        response.Message = "You must select a Role Name to delete.";
        response.CssClass = "red";

        return Json(response);
      }

      _roleService.DeleteRole(roleName);

      response.Success = true;
      response.Message = roleName + " was deleted successfully!";
      response.CssClass = "green";

      return Json(response);
    }

    [HttpPost]
    public ActionResult DeleteRoles(string roles, bool throwOnPopulatedRole)
    {
      var response = new JsonResponse();
      response.Messages = new List<ResponseItem>();

      if (string.IsNullOrEmpty(roles))
      {
        response.Success = false;
        response.Message = "You must select at least one role.";
        return Json(response);
      }

      var roleNames = roles.Split(',');
      var sb = new StringBuilder();

      var validRoleNames = roleNames.Where(role => !string.IsNullOrEmpty(role));
      foreach (var role in validRoleNames)
      {
        ResponseItem item;
        try
        {
          _roleService.DeleteRole(role, throwOnPopulatedRole);

          item = new ResponseItem
          {Success = true, Message = "Deleted this role successfully - " + role, CssClass = "green"};
          response.Messages.Add(item);

          //sb.AppendLine("Deleted this role successfully - " + role + "<br />");
        }
        catch (ProviderException ex)
        {
          //sb.AppendLine(role + " - " + ex.Message + "<br />");

          item = new ResponseItem {Success = false, Message = ex.Message, CssClass = "yellow"};
          response.Messages.Add(item);
        }
      }

      response.Success = true;
      response.Message = sb.ToString();

      return Json(response);
    }

    public ActionResult GetAllRoles()
    {
      var list = _roleService.GetAllRoles();

      var selectList = list.Select(item => new SelectObject {caption = item.Name, value = item.Name}).ToList();

      return Json(selectList, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    public ActionResult GetUsersInRole(string roleName)
    {
      var list = _roleService.GetUsersInRole(roleName);

      return Json(list, JsonRequestBehavior.AllowGet);
    }
  }
}