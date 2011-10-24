using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Contrive.Core.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Core
{
  public class RoleProvider : IRoleProvider
  {
    public RoleProvider(IRoleRepository roles,
                              IUserRepository users,
                              IConfigurationProvider configuration)
    {
      _roles = roles;
      _users = users;
      Initialize(configuration.RoleManagerConfiguration);
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

      var user = _users.FirstOrDefault(u => u.Username == userName);

      if (user == null)
        return false;

      var role = _roles.FirstOrDefault(r => r.Name == roleName);

      return role != null && user.Roles.Contains(role);
    }

    public IEnumerable<IRole> GetAllRoles()
    {
      return Enumerable.ToArray<IRole>(_roles.GetAll());
    }

    public IEnumerable<IUser> GetUsersInRole(string roleName)
    {
      Verify.NotEmpty(roleName, "roleName");

      var role = _roles.FirstOrDefault(r => r.Name == roleName);

      if (role == null)
        throw new InvalidOperationException("Role not found");

      return Enumerable.ToArray<IUser>(role.Users);
    }

    public IEnumerable<IUser> FindUsersInRole(string roleName, string usernameToMatch)
    {
      Verify.NotEmpty(roleName, "roleName");
      Verify.NotEmpty(usernameToMatch, "usernameToMatch");

      var query =
        Queryable.Select(Queryable.Where(Queryable.SelectMany(_roles.GetQuery(), r => r.Users, (r, u) => new { r, u }), @t => @t.r.Name == roleName && @t.u.Username.Contains(usernameToMatch)), @t => @t.u);

      return query.ToArray<IUser>();
    }

    public bool DeleteRole(string roleName, bool throwOnPopulatedRole)
    {
      Verify.NotEmpty(roleName, "roleName");

      var role = _roles.FirstOrDefault(r => r.Name == roleName);

      if (role == null)
        throw new InvalidOperationException("Role not found");

      if (throwOnPopulatedRole)
      {
        if (Enumerable.Any<IUser>(role.Users))
          throw new InvalidOperationException(string.Format("Role populated: {0}", roleName));
      }
      else
        role.Users.Each(u => _users.Delete(u));

      _roles.Delete(role);
      _roles.SaveChanges();

      return true;
    }

    public IEnumerable<IRole> GetRolesForUser(string userName)
    {
      Verify.NotEmpty(userName, "userName");

      var user = _users.FirstOrDefault(u => u.Username == userName);

      if (user == null)
        throw new InvalidOperationException(string.Format("User not found: {0}", userName));

      return Enumerable.ToArray<IRole>(user.Roles);
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
      var users = Enumerable.ToList<IUser>(_users.Where(u => userNames.Contains(u.Username)));
      var roles = Enumerable.ToList<IRole>(_roles.Where(r => roleNames.Contains(r.Name)));

      users.Each(u =>
      {
        var availableRoles = roles.Where(role => !u.Roles.Contains(role));
        availableRoles.Each(u.Roles.Add);
      });

      _roles.SaveChanges();
    }

    public void RemoveUsersFromRoles(string[] userNames, string[] roleNames)
    {
      var users = userNames
        .Select<string, IUser>(name => _users.FirstOrDefault(u => u.Username == name))
        .Where(u => u != null);

      users.Each(user =>
      {
        var roles = roleNames
          .Select(n => Enumerable.FirstOrDefault<IRole>(user.Roles, r => r.Name == n))
          .Where(r => r != null);

        roles.Each(r => user.Roles.Remove(r));
      });

      _roles.SaveChanges();
    }

    void Initialize(NameValueCollection config)
    {
      Verify.NotNull(config, "config");

      if (string.IsNullOrEmpty(config["description"]))
      {
        config.Remove("description");
        config.Add("description", "Role Provider");
      }

      ApplicationName = config["applicationName"];
    }
  }
}