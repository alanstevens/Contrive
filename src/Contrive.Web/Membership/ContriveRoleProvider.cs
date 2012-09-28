using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Linq;
using System.Web.Security;
using Contrive.Core;
using Contrive.Core.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Web.Membership
{
  public class ContriveRoleProvider : RoleProvider
  {
    public override string ApplicationName { get; set; }

    IRoleService GetRoleManagementService()
    {
      return ServiceLocator.Current.GetInstance<IRoleService>();
    }

    public override void Initialize(string name, NameValueCollection config)
    {
      Verify.NotNull(config, "config");

      if (name.IsEmpty()) name = "ContriveRoleProvider";

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
      return GetRoleManagementService().GetAllRoles().Select(r => r.Name).ToArray();
    }

    public override string[] GetUsersInRole(string roleName)
    {
      string[] usersInRole = null;

      ThrowMembership(() => usersInRole = GetRoleManagementService().GetUsersInRole(roleName).Select(u => u.UserName).ToArray());

      return usersInRole;
    }

    public override string[] FindUsersInRole(string roleName, string usernameToMatch)
    {
      string[] usersInRole = null;

      ThrowMembership(() => usersInRole = GetRoleManagementService().FindUsersInRole(roleName, usernameToMatch).Select(u => u.UserName).ToArray());

      return usersInRole;
    }

    public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
    {
      var success = false;

      ThrowMembership(() => success = GetRoleManagementService().DeleteRole(roleName, throwOnPopulatedRole));

      return success;
    }

    public override string[] GetRolesForUser(string userName)
    {
      string[] rolesForUser = null;

      ThrowMembership(() => rolesForUser = GetRoleManagementService().GetRolesForUser(userName).Select(r => r.Name).ToArray());

      return rolesForUser;
    }

    public override void CreateRole(string roleName)
    {
      ThrowMembership(() => GetRoleManagementService().CreateRole(roleName));
    }

    public override void AddUsersToRoles(string[] userNames, string[] roleNames)
    {
      GetRoleManagementService().AddUsersToRoles(userNames, roleNames);
    }

    public override void RemoveUsersFromRoles(string[] userNames, string[] roleNames)
    {
      GetRoleManagementService().RemoveUsersFromRoles(userNames, roleNames);
    }

    void ThrowMembership(Action test)
    {
      try
      {
        test();
      }
      catch (InvalidOperationException e)
      {
        throw new ProviderException(e.Message, e);
      }
    }
  }
}