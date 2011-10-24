using System.Collections.Generic;

namespace Contrive.Core
{
  public interface IRoleProvider
  {
    string ApplicationName { get; set; }

    bool RoleExists(string roleName);

    bool IsUserInRole(string userName, string roleName);

    IEnumerable<IRole> GetAllRoles();

    IEnumerable<IUser> GetUsersInRole(string roleName);

    IEnumerable<IUser> FindUsersInRole(string roleName, string usernameToMatch);

    bool DeleteRole(string roleName, bool throwOnPopulatedRole);

    IEnumerable<IRole> GetRolesForUser(string userName);

    void CreateRole(string roleName);

    void AddUsersToRoles(string[] userNames, string[] roleNames);

    void RemoveUsersFromRoles(string[] userNames, string[] roleNames);
  }
}