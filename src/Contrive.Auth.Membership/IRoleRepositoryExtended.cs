using System.Collections.Generic;

namespace Contrive.Auth.Membership
{
    public interface IRoleRepositoryExtended
    {
        IEnumerable<IRoleExtended> GetAll();

        IRoleExtended GetRoleByName(string roleName);

        IEnumerable<IUserExtended> FindUsersInRole(string roleName, string usernameToMatch);

        IEnumerable<IRoleExtended> GetRolesForRoleNames(IEnumerable<string> roleNames);

        void Insert(IRoleExtended roleExtended);

        void Update(IRoleExtended roleExtended);

        void Delete(IRoleExtended roleExtended);
    }
}