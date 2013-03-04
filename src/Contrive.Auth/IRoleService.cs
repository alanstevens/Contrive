using System.Collections.Generic;

namespace Contrive.Auth
{
    public interface IRoleService
    {
        bool RoleExists(string roleName);

        bool IsUserInRole(string userName, string roleName);

        IEnumerable<IRole> GetAllRoles();

        IEnumerable<IUser> GetUsersInRole(string roleName);

        IEnumerable<IUser> FindUsersInRole(string roleName, string usernameToMatch);

        bool DeleteRole(string roleName, bool throwOnPopulatedRole = false);

        IEnumerable<IRole> GetRolesForUser(string userName);

        void CreateRole(string roleName);

        void AddUsersToRoles(string[] users, string[] roles);

        void RemoveUsersFromRoles(string[] users, string[] roles);

        IEnumerable<IRole> GetRolesForUser(IUser user);
    }
}