using System;
using System.Collections.Specialized;
using System.Linq;
using Contrive.Core;
using Microsoft.Practices.ServiceLocation;
using RoleProvider = System.Web.Security.RoleProvider;

namespace Contrive.Web.Membership
{
  public class ContriveRoleProvider : RoleProvider
  {
    public override string ApplicationName { get; set; }

    IRoleProvider GetRoleManagementService()
    {
      return ServiceLocator.Current.GetInstance<IRoleProvider>();
    }

    public override void Initialize(string name, NameValueCollection config)
    {
      if (config == null)
        throw new ArgumentNullException("config");

      if (string.IsNullOrEmpty(name))
        name = "EntityRoleProvider";

      if (string.IsNullOrEmpty(config["description"]))
      {
        config.Remove("description");
        config.Add("description", "Entity Role Provider");
      }

      base.Initialize(name, config);

      ApplicationName = GetRoleManagementService().ApplicationName;
    }

    public override bool RoleExists(string roleName)
    {
      return GetRoleManagementService().RoleExists(roleName);
    }

    public override bool IsUserInRole(string userName, string roleName)
    {
      return GetRoleManagementService().IsUserInRole(userName, roleName);
    }

    public override string[] GetAllRoles()
    {
      return GetRoleManagementService()
        .GetAllRoles()
        .Select(r => r.Name).ToArray();
    }

    public override string[] GetUsersInRole(string roleName)
    {
      return GetRoleManagementService()
        .GetUsersInRole(roleName)
        .Select(u => u.Username).ToArray();
    }

    public override string[] FindUsersInRole(string roleName, string usernameToMatch)
    {
      return GetRoleManagementService()
        .FindUsersInRole(roleName, usernameToMatch)
        .Select(u => u.Username).ToArray();
    }

    public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
    {
      return GetRoleManagementService().DeleteRole(roleName, throwOnPopulatedRole);
    }

    public override string[] GetRolesForUser(string userName)
    {
      return GetRoleManagementService()
        .GetRolesForUser(userName)
        .Select(r => r.Name).ToArray();
    }

    public override void CreateRole(string roleName)
    {
      GetRoleManagementService().CreateRole(roleName);
    }

    public override void AddUsersToRoles(string[] userNames, string[] roleNames)
    {
      GetRoleManagementService().AddUsersToRoles(userNames, roleNames);
    }

    public override void RemoveUsersFromRoles(string[] userNames, string[] roleNames)
    {
      GetRoleManagementService().RemoveUsersFromRoles(userNames, roleNames);
    }
  }
}