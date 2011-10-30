using System;
using System.Collections.Generic;
using System.Linq;
using Contrive.Core.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Core
{
  public class RoleService : IRoleService
  {
    public RoleService(IRoleRepository roles,
                       IUserRepository users,
                       IConfigurationProvider configuration)
    {
      _roles = roles;
      _users = users;

      var config = configuration.RoleServiceConfiguration;

      Verify.NotNull(config, "config");

      if (string.IsNullOrEmpty(config["description"]))
      {
        config.Remove("description");
        config.Add("description", "Contrive Role Provider");
      }

      ApplicationName = config["applicationName"];
    }

    readonly IRoleRepository _roles;
    readonly IUserRepository _users;

    public string ApplicationName { get; set; }

    public bool RoleExists(string roleName)
    {
      Verify.NotEmpty(roleName, "roleName");

      var result = _roles.FirstOrDefault(r => r.Name == roleName);

      return result != null;
    }

    public bool IsUserInRole(string userName, string roleName)
    {
      Verify.NotEmpty(userName, "userName");
      Verify.NotEmpty(roleName, "roleName");

      var user = _users.FirstOrDefault(u => u.UserName == userName);

      if (user == null)
        return false;

      var role = _roles.FirstOrDefault(r => r.Name == roleName);

      return role != null && user.Roles.Contains(role);
    }

    public IEnumerable<IRole> GetAllRoles()
    {
      return _roles.GetAll();
    }

    public IEnumerable<IUser> GetUsersInRole(string roleName)
    {
      Verify.NotEmpty(roleName, "roleName");

      var role = VerifyRole(roleName);

      return role.Users;
    }

    public IEnumerable<IUser> FindUsersInRole(string roleName, string usernameToMatch)
    {
      Verify.NotEmpty(roleName, "roleName");
      Verify.NotEmpty(usernameToMatch, "usernameToMatch");

      VerifyRole(roleName);

      var query = _roles.GetQuery().SelectMany(r => r.Users, (r, u) => new { r, u });

      return query.Where(t => t.r.Name == roleName && t.u.UserName.Contains(usernameToMatch))
        .Select(t => t.u).ToArray();
    }

    public bool DeleteRole(string roleName, bool throwOnPopulatedRole = false)
    {
      Verify.NotEmpty(roleName, "roleName");

      var role = VerifyRole(roleName);

      if (throwOnPopulatedRole)
      {
        if (role.Users.Any())
          throw new InvalidOperationException(string.Format("Role populated: {0}", roleName));
      }
      else
      {
        role.Users.Each(u =>
        {
          u.Roles.Remove(role);
          _users.Update(u);
        });
      }

      _roles.Delete(role);
      _roles.SaveChanges();

      return true;
    }

    public IEnumerable<IRole> GetRolesForUser(string userName)
    {
      Verify.NotEmpty(userName, "userName");

      var user = VerifyUser(userName);

      return user.Roles;
    }

    public void CreateRole(string roleName)
    {
      Verify.NotEmpty(roleName, "roleName");

      var role = _roles.FirstOrDefault(r => r.Name == roleName);

      if (role != null)
        throw new InvalidOperationException(string.Format("Role exists: {0}", roleName));

      var newRole = ServiceLocator.Current.GetInstance<IRole>();
      newRole.Id = Guid.NewGuid();
      newRole.Name = roleName;

      _roles.Insert(newRole);
      _roles.SaveChanges();
    }

    public void AddUsersToRoles(string[] userNames, string[] roleNames)
    {
      AddUsersToRoles(GetUsersForUserNames(userNames), GetRolesForRoleNames(roleNames));
    }

    public void RemoveUsersFromRoles(string[] userNames, string[] roleNames)
    {
      RemoveUsersFromRoles(GetUsersForUserNames(userNames), GetRolesForRoleNames(roleNames));
    }

    public void AddUsersToRoles(IEnumerable<IUser> users, IEnumerable<IRole> roles)
    {
      foreach (var u in users)
      {
        var availableRoles = roles.Where(role => !u.Roles.Contains(role)).ToArray();

        availableRoles.Each(r =>
        {
          u.Roles.Add(r);
          r.Users.Add(u);
          _roles.Update(r);
        });
        _users.Update(u);
      }
      _roles.SaveChanges();
    }

    public void RemoveUsersFromRoles(IEnumerable<IUser> users, IEnumerable<IRole> roles)
    {
      foreach (var u in users)
      {
        var availableRoles = roles.Where(role => u.Roles.Contains(role)).ToArray();

        availableRoles.Each(r =>
        {
          u.Roles.Remove(r);
          r.Users.Remove(u);
          _roles.Update(r);
        });
        _users.Update(u);
      }

      _roles.SaveChanges();
    }

    IEnumerable<IRole> GetRolesForRoleNames(IEnumerable<string> roleNames)
    {
      return _roles.Where(r => roleNames.Contains(r.Name)).ToArray();
    }

    IEnumerable<IUser> GetUsersForUserNames(IEnumerable<string> userNames)
    {
      return _users.Where(u => userNames.Contains(u.UserName)).ToArray();
    }

    IRole VerifyRole(string roleName)
    {
      var role = _roles.FirstOrDefault(r => r.Name == roleName);

      if (role == null)
        throw new InvalidOperationException("Role not found");
      return role;
    }

    IUser VerifyUser(string userName)
    {
      var user = _users.FirstOrDefault(u => u.UserName == userName);

      if (user == null)
        throw new InvalidOperationException(string.Format("User not found: {0}", userName));
      return user;
    }
  }
}