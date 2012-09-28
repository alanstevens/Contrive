using System.Collections.Generic;

namespace Contrive.Auth
{
  public interface IRoleRepository
  {
    IEnumerable<IRole> GetAll();

    IRole GetRoleByName(string roleName);

    IEnumerable<IUser> FindUsersInRole(string roleName, string usernameToMatch);

    IEnumerable<IRole> GetRolesForRoleNames(IEnumerable<string> roleNames);

    void Insert(IRole role);

    void Update(IRole role);

    void Delete(IRole role);
  }
}