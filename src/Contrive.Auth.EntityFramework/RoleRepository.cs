using System.Collections.Generic;
using System.Linq;
using Contrive.Auth.Membership;
using Contrive.Common.Extensions;

namespace Contrive.Auth.EntityFramework
{
  public class RoleRepository : IRoleRepository
  {
    public RoleRepository(Repository<Role> repository)
    {
      _repository = repository;
    }

    readonly Repository<Role> _repository;

    public IEnumerable<IRole> GetAll()
    {
      return _repository.GetAll();
    }

    public IRole GetRoleByName(string roleName)
    {
      return _repository.FirstOrDefault(r => r.Name == roleName);
    }

    public IEnumerable<IUserExtended> FindUsersInRole(string roleName, string usernameToMatch)
    {
      var usersInRole = GetRoleByName(roleName).Users;

      return usersInRole.Where(u => u.UserName.Contains(usernameToMatch));
    }

    public IEnumerable<IRole> GetRolesForRoleNames(IEnumerable<string> roleNames)
    {
      return _repository.Where(r => roleNames.Contains(r.Name)).ToArray();
    }

    public void Insert(IRole role)
    {
      _repository.Insert(role.As<Role>());
      SaveChanges();
    }

    public void Update(IRole role)
    {
      _repository.Update(role.As<Role>());
      SaveChanges();
    }

    public void Delete(IRole role)
    {
      _repository.Delete(role.As<Role>());
      SaveChanges();
    }

    void SaveChanges()
    {
      _repository.SaveChanges();
    }
  }
}