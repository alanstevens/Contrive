using System.Collections.Generic;
using Contrive.Auth;
using Contrive.Common.Data;

namespace Contrive.Sample.Data
{
    public class RoleRepository: DataServiceBase, IRoleRepository
    {
        public IEnumerable<IRole> GetAll()
        {
            yield break;
        }

        public IRole GetRoleByName(string roleName)
        {
            return null;
        }

        public IEnumerable<IUser> FindUsersInRole(string roleName, string usernameToMatch)
        {
            yield break;
        }

        public IEnumerable<IRole> GetRolesForRoleNames(IEnumerable<string> roleNames)
        {
            yield break;
        }

        public void Insert(IRole role) {}

        public void Update(IRole role) {}

        public void Delete(IRole role) {}
    }
}