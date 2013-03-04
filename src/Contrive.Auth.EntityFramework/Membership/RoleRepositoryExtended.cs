using System.Collections.Generic;
using System.Linq;
using Contrive.Auth.Membership;
using Contrive.Common.Extensions;

namespace Contrive.Auth.EntityFramework.Membership
{
    public class RoleRepositoryExtended : IRoleRepositoryExtended
    {
        public RoleRepositoryExtended(Repository<RoleExtended> repository)
        {
            _repository = repository;
        }

        readonly Repository<RoleExtended> _repository;

        public IEnumerable<IRoleExtended> GetAll()
        {
            return _repository.GetAll();
        }

        public IRoleExtended GetRoleByName(string roleName)
        {
            return _repository.FirstOrDefault(r => r.Name == roleName);
        }

        public IEnumerable<IUserExtended> FindUsersInRole(string roleName, string usernameToMatch)
        {
            var usersInRole = GetRoleByName(roleName).Users;

            return usersInRole.Where(u => u.UserName.Contains(usernameToMatch));
        }

        public IEnumerable<IRoleExtended> GetRolesForRoleNames(IEnumerable<string> roleNames)
        {
            return _repository.Where(r => roleNames.Contains(r.Name)).ToArray();
        }

        public void Insert(IRoleExtended roleExtended)
        {
            _repository.Insert(roleExtended.As<RoleExtended>());
            SaveChanges();
        }

        public void Update(IRoleExtended roleExtended)
        {
            _repository.Update(roleExtended.As<RoleExtended>());
            SaveChanges();
        }

        public void Delete(IRoleExtended roleExtended)
        {
            _repository.Delete(roleExtended.As<RoleExtended>());
            SaveChanges();
        }

        void SaveChanges()
        {
            _repository.SaveChanges();
        }
    }
}