using System;
using System.Collections.Generic;
using System.Linq;
using Contrive.Common;
using Contrive.Common.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Auth.Membership
{
  public class RoleService : IRoleService
  {
    public RoleService(IRoleRepository roleRepository,
                       IUserExtendedRepository userRepository,
                       IMembershipConfigurationProvider configuration)
    {
      _roleRepository = roleRepository;
      _userRepository = userRepository;

      var config = configuration.RoleServiceConfiguration;

      Verify.NotNull(config, "config");

      if (string.IsNullOrEmpty(config["description"]))
      {
        config.Remove("description");
        config.Add("description", "Contrive Role Provider");
      }

      ApplicationName = config["applicationName"];
    }

    readonly IRoleRepository _roleRepository;
    readonly IUserExtendedRepository _userRepository;

    public string ApplicationName { get; set; }

    public bool RoleExists(string roleName)
    {
      Verify.NotEmpty(roleName, "roleName");

      var role = _roleRepository.GetRoleByName(roleName);

      return role != null;
    }

    public bool IsUserInRole(string userName, string roleName)
    {
      Verify.NotEmpty(userName, "userName");
      Verify.NotEmpty(roleName, "roleName");

      var user = _userRepository.GetUserByUserName(userName);

      if (user == null) return false;

      var role = _roleRepository.GetRoleByName(roleName);

      return role != null && user.Roles.Contains(role);
    }

    public IEnumerable<IRole> GetAllRoles()
    {
      return _roleRepository.GetAll();
    }

    public IEnumerable<IUserExtended> GetUsersInRole(string roleName)
    {
      Verify.NotEmpty(roleName, "roleName");

      var role = VerifyRole(roleName);

      return role.Users;
    }

    public IEnumerable<IUserExtended> FindUsersInRole(string roleName, string usernameToMatch)
    {
      Verify.NotEmpty(roleName, "roleName");
      Verify.NotEmpty(usernameToMatch, "usernameToMatch");
      return _roleRepository.FindUsersInRole(roleName, usernameToMatch);
    }

    public bool DeleteRole(string roleName, bool throwOnPopulatedRole = false)
    {
      Verify.NotEmpty(roleName, "roleName");

      var role = VerifyRole(roleName);

      if (throwOnPopulatedRole)
      {
        if (role.Users.Any()) throw new InvalidOperationException(string.Format("Role populated: {0}", roleName));
      }
      else
      {
        role.Users.Each(u =>
                        {
                          u.Roles.Remove(role);
                          _userRepository.Update(u);
                        });
      }

      _roleRepository.Delete(role);

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

      var role = _roleRepository.GetRoleByName(roleName);

      if (role != null) throw new InvalidOperationException(string.Format("Role exists: {0}", roleName));

      var newRole = ServiceLocator.Current.GetInstance<IRole>();
      newRole.Id = Guid.NewGuid();
      newRole.Name = roleName;

      _roleRepository.Insert(newRole);
    }

    public void AddUsersToRoles(string[] users, string[] roles)
    {
      Verify.NotEmpty(users,"users");
      Verify.NotEmpty(roles,"roles");

      AddUsersToRoles(GetUsersForUserNames(users), GetRolesForRoleNames(roles));
    }

    public void RemoveUsersFromRoles(string[] users, string[] roles)
    {
      Verify.NotEmpty(users,"users");
      Verify.NotEmpty(roles,"roles");

      RemoveUsersFromRoles(GetUsersForUserNames(users), GetRolesForRoleNames(roles));
    }

    public void AddUsersToRoles(IEnumerable<IUserExtended> users, IEnumerable<IRole> roles)
    {
      Verify.NotEmpty(users,"users");
      Verify.NotEmpty(roles,"roles");

      foreach (var u in users)
      {
        var availableRoles = roles.Where(role => !u.Roles.Contains(role)).ToArray();

        availableRoles.Each(r =>
                            {
                              u.Roles.Add(r);
                              r.Users.Add(u);
                              _roleRepository.Update(r);
                            });
        _userRepository.Update(u);
      }
    }

    public void RemoveUsersFromRoles(IEnumerable<IUserExtended> users, IEnumerable<IRole> roles)
    {
      Verify.NotEmpty(users,"users");
      Verify.NotEmpty(roles,"roles");

      foreach (var u in users)
      {
        var availableRoles = roles.Where(role => u.Roles.Contains(role)).ToArray();

        availableRoles.Each(r =>
                            {
                              u.Roles.Remove(r);
                              r.Users.Remove(u);
                              _roleRepository.Update(r);
                            });

        _userRepository.Update(u);
      }
    }

    IEnumerable<IRole> GetRolesForRoleNames(IEnumerable<string> roles)
    {
      return _roleRepository.GetRolesForRoleNames(roles).ToArray();
    }

    IEnumerable<IUserExtended> GetUsersForUserNames(IEnumerable<string> users)
    {
      return _userRepository.GetUsersForUserNames(users);
    }

    IRole VerifyRole(string roleName)
    {
      var role = _roleRepository.GetRoleByName(roleName);

      if (role == null) throw new InvalidOperationException("Role not found");
      return role;
    }

    IUserExtended VerifyUser(string userName)
    {
      var user = _userRepository.GetUserByUserName(userName);

      if (user == null) throw new InvalidOperationException(string.Format("User not found: {0}", userName));
      return user;
    }
  }
}