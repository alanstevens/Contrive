using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Text;
using System.Web.Mvc;
using Contrive.Core;
using Contrive.Sample.Areas.Contrive.Models;

namespace Contrive.Sample.Areas.Contrive.Controllers
{
  [Authorize(Roles = "SecurityGuard")]
  public class RoleController : Controller
  {
    public RoleController(IRoleService roleService)
    {
      _roleService = roleService;
    }

    readonly IRoleService _roleService;

    public virtual ActionResult Index()
    {
      ManageRolesViewModel model = new ManageRolesViewModel();
      model.Roles = new SelectList(_roleService.GetAllRoles());
      model.RoleList = _roleService.GetAllRoles();

      return View(model);
    }

    [HttpGet]
    public virtual ActionResult CreateRole()
    {
      return View(new RoleViewModel());
    }

    [HttpPost]
    public virtual ActionResult CreateRole(string roleName)
    {
      JsonResponse response = new JsonResponse();

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

    /// <summary>
    ///   This is an Ajax method.
    /// </summary>
    /// <param name = "roleName"></param>
    /// <returns></returns>
    [HttpPost]
    public virtual ActionResult DeleteRole(string roleName)
    {
      JsonResponse response = new JsonResponse();

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
      JsonResponse response = new JsonResponse();
      response.Messages = new List<ResponseItem>();

      if (string.IsNullOrEmpty(roles))
      {
        response.Success = false;
        response.Message = "You must select at least one role.";
        return Json(response);
      }

      string[] roleNames = roles.Split(',');
      StringBuilder sb = new StringBuilder();

      ResponseItem item = null;

      foreach (var role in roleNames)
      {
        if (!string.IsNullOrEmpty(role))
        {
          try
          {
            _roleService.DeleteRole(role, throwOnPopulatedRole);

            item = new ResponseItem();
            item.Success = true;
            item.Message = "Deleted this role successfully - " + role;
            item.CssClass = "green";
            response.Messages.Add(item);

            //sb.AppendLine("Deleted this role successfully - " + role + "<br />");
          }
          catch (ProviderException ex)
          {
            //sb.AppendLine(role + " - " + ex.Message + "<br />");

            item = new ResponseItem();
            item.Success = false;
            item.Message = ex.Message;
            item.CssClass = "yellow";
            response.Messages.Add(item);
          }
        }
      }

      response.Success = true;
      response.Message = sb.ToString();

      return Json(response);
    }

    /// <summary>
    ///   This is an Ajax method that populates the 
    ///   Roles drop down list.
    /// </summary>
    /// <returns></returns>
    public ActionResult GetAllRoles()
    {
      var list = _roleService.GetAllRoles();

      List<SelectObject> selectList = new List<SelectObject>();

      foreach (var item in list)
      {
        selectList.Add(new SelectObject {caption = item.Name, value = item.Name});
      }

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