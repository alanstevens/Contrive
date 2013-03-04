using System;
using System.Collections.Generic;
using System.Linq;
using Contrive.Common;
using Contrive.Common.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Auth.Membership
{
    public class RoleServiceExtended : IRoleServiceExtended
    {
        public RoleServiceExtended(IRoleRepositoryExtended roleRepositoryExtended,
                           IUserExtendedRepository userRepository,
                           IAuthConfigurationProvider configuration)
        {
            _roleRepositoryExtended = roleRepositoryExtended;
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

        readonly IRoleRepositoryExtended _roleRepositoryExtended;
        readonly IUserExtendedRepository _userRepository;

        public string ApplicationName { get; set; }

        public bool RoleExists(string roleName)
        {
            Verify.NotEmpty(roleName, "roleName");

            var role = _roleRepositoryExtended.GetRoleByName(roleName);

            return role != null;
        }

        public bool IsUserInRole(string userName, string roleName)
        {
            Verify.NotEmpty(userName, "userName");
            Verify.NotEmpty(roleName, "roleName");

            var user = _userRepository.GetUserByUserName(userName);

            if (user == null) return false;

            var role = _roleRepositoryExtended.GetRoleByName(roleName);

            return role != null && user.Roles.Contains(role);
        }

        public IEnumerable<IRoleExtended> GetAllRoles()
        {
            return _roleRepositoryExtended.GetAll();
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
            return _roleRepositoryExtended.FindUsersInRole(roleName, usernameToMatch);
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

            _roleRepositoryExtended.Delete(role);

            return true;
        }

        public IEnumerable<IRoleExtended> GetRolesForUser(string userName)
        {
            Verify.NotEmpty(userName, "userName");

            var user = VerifyUser(userName);

            return user.Roles;
        }

        public void CreateRole(string roleName)
        {
            Verify.NotEmpty(roleName, "roleName");

            var role = _roleRepositoryExtended.GetRoleByName(roleName);

            if (role != null) throw new InvalidOperationException(string.Format("Role exists: {0}", roleName));

            var newRole = ServiceLocator.Current.GetInstance<IRoleExtended>();
            newRole.Id = Guid.NewGuid();
            newRole.Name = roleName;

            _roleRepositoryExtended.Insert(newRole);
        }

        public void AddUsersToRoles(string[] users, string[] roles)
        {
            Verify.NotEmpty(users, "users");
            Verify.NotEmpty(roles, "roles");

            AddUsersToRoles(GetUsersForUserNames(users), GetRolesForRoleNames(roles));
        }

        public void RemoveUsersFromRoles(string[] users, string[] roles)
        {
            Verify.NotEmpty(users, "users");
            Verify.NotEmpty(roles, "roles");

            RemoveUsersFromRoles(GetUsersForUserNames(users), GetRolesForRoleNames(roles));
        }

        public void AddUsersToRoles(IEnumerable<IUserExtended> users, IEnumerable<IRoleExtended> roles)
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
                                        _roleRepositoryExtended.Update(r);
                                    });
                _userRepository.Update(u);
            }
        }

        public void RemoveUsersFromRoles(IEnumerable<IUserExtended> users, IEnumerable<IRoleExtended> roles)
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
                                        _roleRepositoryExtended.Update(r);
                                    });

                _userRepository.Update(u);
            }
        }

        IEnumerable<IRoleExtended> GetRolesForRoleNames(IEnumerable<string> roles)
        {
            return _roleRepositoryExtended.GetRolesForRoleNames(roles).ToArray();
        }

        IEnumerable<IUserExtended> GetUsersForUserNames(IEnumerable<string> users)
        {
            return _userRepository.GetUsersForUserNames(users);
        }

        IRoleExtended VerifyRole(string roleName)
        {
            var role = _roleRepositoryExtended.GetRoleByName(roleName);

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