using System;
using System.Collections.Generic;
using System.Linq;
using Contrive.Common;
using Contrive.Common.Extensions;

namespace Contrive.Auth
{
    public class RoleService : IRoleService
    {
        public RoleService(IRoleRepository roleRepository,
                           IUserRepository userRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
        }

        public static Func<IRole> NewRole = () => new Role();

        readonly IRoleRepository _roleRepository;
        readonly IUserRepository _userRepository;

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

        public IEnumerable<IUser> GetUsersInRole(string roleName)
        {
            Verify.NotEmpty(roleName, "roleName");

            var role = VerifyRole(roleName);

            return role.Users;
        }

        public IEnumerable<IUser> FindUsersInRole(string roleName, string usernameToMatch)
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

        public IEnumerable<IRole> GetRolesForUser(IUser user)
        {
            Verify.NotNull(user, "user");

            return GetRolesForUser(user.UserName);
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

            var newRole = NewRole();
            newRole.Id = Guid.NewGuid();
            newRole.Name = roleName;

            _roleRepository.Insert(newRole);
        }

        public void AddUsersToRoles(string[] users, string[] roles)
        {
            Verify.NotEmpty(users, "users");
            Verify.NotEmpty(roles, "roles");

            AddUsersToRoles(_userRepository.GetUsersForUserNames(users), GetRolesForRoleNames(roles));
        }

        public void RemoveUsersFromRoles(string[] users, string[] roles)
        {
            Verify.NotEmpty(users, "users");
            Verify.NotEmpty(roles, "roles");

            RemoveUsersFromRoles(_userRepository.GetUsersForUserNames(users), GetRolesForRoleNames(roles));
        }

        public void AddUsersToRoles(IEnumerable<IUser> users, IEnumerable<IRole> roles)
        {
            Verify.NotEmpty(users, "users");
            Verify.NotEmpty(roles, "roles");

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

        public void RemoveUsersFromRoles(IEnumerable<IUser> users, IEnumerable<IRole> roles)
        {
            Verify.NotEmpty(users, "users");
            Verify.NotEmpty(roles, "roles");

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

        IRole VerifyRole(string roleName)
        {
            var role = _roleRepository.GetRoleByName(roleName);

            if (role == null) throw new InvalidOperationException("Role not found");
            return role;
        }

        IUser VerifyUser(string userName)
        {
            var user = _userRepository.GetUserByUserName(userName);

            if (user == null) throw new InvalidOperationException(string.Format("User not found: {0}", userName));
            return user;
        }
    }
}