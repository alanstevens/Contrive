using System.Collections.Generic;

namespace Contrive.Auth.Membership
{
    public interface IRoleServiceExtended
    {
        string ApplicationName { get; set; }

        bool RoleExists(string roleName);

        bool IsUserInRole(string userName, string roleName);

        IEnumerable<IRoleExtended> GetAllRoles();

        IEnumerable<IUserExtended> GetUsersInRole(string roleName);

        IEnumerable<IUserExtended> FindUsersInRole(string roleName, string usernameToMatch);

        bool DeleteRole(string roleName, bool throwOnPopulatedRole = false);

        IEnumerable<IRoleExtended> GetRolesForUser(string userName);

        void CreateRole(string roleName);

        void AddUsersToRoles(string[] users, string[] roles);

        void RemoveUsersFromRoles(string[] users, string[] roles);
    }
}