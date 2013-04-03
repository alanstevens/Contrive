using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Contrive.Auth;
using Contrive.Auth.Membership;
using Contrive.Tests.SubSpec;
using FluentAssertions;
using Microsoft.Practices.ServiceLocation;
using Moq;

namespace Contrive.Tests
{
    public class Using_RoleServiceSpecs
    {
        public Using_RoleServiceSpecs()
        {
            _configuration.SetupGet(c => c.RoleServiceConfiguration).Returns(new NameValueCollection());
            _roleServiceExtended = new RoleServiceExtended(_roleRepository.Object, _userRepository.Object, _configuration.Object);
        }

        readonly Mock<IAuthConfigurationProvider> _configuration = new Mock<IAuthConfigurationProvider>();
        readonly Mock<IRoleExtended> _roleMock2 = new Mock<IRoleExtended>();
        readonly Mock<IRoleRepositoryExtended> _roleRepository = new Mock<IRoleRepositoryExtended>();
        readonly RoleServiceExtended _roleServiceExtended;
        readonly Mock<IUserExtended> _userMock2 = new Mock<IUserExtended>();
        readonly Mock<IUserExtendedRepository> _userRepository = new Mock<IUserExtendedRepository>();
        readonly Mock<IRoleExtended> roleMock = new Mock<IRoleExtended>();
        readonly Mock<IUserExtended> userMock = new Mock<IUserExtended>();
        string roleName = "admin";
        string userName = "someUser";

        [Specification]
        public void Get_all_should_not_be_empty()
        {
            "Given a new RoleService".Context(
                                              () =>
                                              _roleRepository.Setup(r => r.GetAll()).Returns(new List<IRoleExtended>
                                                                                             {new Mock<IRoleExtended>().Object}));
            "Get all should not be empty".Assert(() => _roleServiceExtended.GetAllRoles().Should().NotBeEmpty());
        }

        [Specification]
        public void When_checing_users_in_a_role()
        {
            var userInRole = false;
            "Given ".Context(() =>
                             {
                                 SetupUser();
                                 SetupRole();
                                 _roleRepository.Setup(rr => rr.GetRoleByName(It.IsAny<string>())).Returns(roleMock.Object);
                                 _userRepository.Setup(ur => ur.GetUserByUserName(It.IsAny<string>())).Returns(userMock.Object);
                             });
            "When ".Do(() => userInRole = _roleServiceExtended.IsUserInRole(userName, roleName));
            "It should ".Assert(() => { userInRole.Should().BeTrue(); });
        }

        [Specification]
        public void When_checking_if_role_exists()
        {
            var roleExists = false;
            "Given ".Context(() =>
                             {
                                 SetupRole();
                                 _roleRepository.Setup(rr => rr.GetRoleByName(It.IsAny<string>())).Returns(roleMock.Object);
                             });
            "When ".Do(() => roleExists = _roleServiceExtended.RoleExists(roleName));
            "It should ".Assert(() => roleExists.Should().BeTrue());
        }

        [Specification]
        public void When_checking_if_user_is_in_a_role()
        {
            SetupUser();
            SetupRole();
            var userIsInRole = false;
            "Given ".Context(() =>
                             {
                                 _roleRepository.Setup(rr => rr.GetRoleByName(It.IsAny<string>())).Returns(roleMock.Object);
                                 _userRepository.Setup(ur => ur.GetUserByUserName(It.IsAny<string>())).Returns(userMock.Object);
                             });
            "When ".Do(() => userIsInRole = _roleServiceExtended.IsUserInRole(userName, roleName));
            "It should ".Assert(() => userIsInRole.Should().BeTrue());
        }

        [Specification]
        public void When_finding_roles_matching_a_username_pattern()
        {
            SetupUser();
            SetupRole();
            IEnumerable<IUserExtended> usersInRole = null;
            "Given ".Context(
                             () => { _roleRepository.Setup(rr => rr.GetRoleByName(It.IsAny<string>())).Returns(roleMock.Object); });
            "When ".Do(() => usersInRole = _roleServiceExtended.FindUsersInRole(roleName, userName.Substring(0, 4)));
            "It should ".Assert(() => usersInRole.Contains(userMock.Object));
            "It should ".Assert(() => usersInRole.Count().Should().Be(1));
        }

        [Specification]
        public void When_deleting_a_role()
        {
            SetupUser();
            SetupRole();
            "Given ".Context(() => _roleRepository.Setup(rr => rr.GetRoleByName(It.IsAny<string>())).Returns(roleMock.Object));
            "When ".Do(() => _roleServiceExtended.DeleteRole(roleName, false));
            "It should ".Assert(() => userMock.Object.Roles.Should().NotContain(roleMock.Object));
        }

        [Specification]
        public void When_getting_roles_for_a_user()
        {
            SetupUser();
            IEnumerable<IRoleExtended> roles = null;
            "Given ".Context(
                             () =>
                             _userRepository.Setup(ur => ur.GetUserByUserName(It.IsAny<string>())).Returns(userMock.Object));
            "When ".Do(() => roles = _roleServiceExtended.GetRolesForUser(userName));
            "It should ".Assert(() => roles.Should().Contain(roleMock.Object));
            "It should ".Assert(() => roles.Count().Should().Be(1));
        }

        [Specification]
        public void When_creating_a_role()
        {
            "Given ".Context(() =>
                             {
                                 var serviceLocator = new Mock<IServiceLocator>();
                                 serviceLocator.Setup(sl => sl.GetInstance<IRoleExtended>()).Returns(roleMock.Object);
                                 ServiceLocator.SetLocatorProvider(() => serviceLocator.Object);
                                 roleMock.SetupProperty(r => r.Name);
                             });
            "When ".Do(() => _roleServiceExtended.CreateRole("foo"));
            "It should ".Assert(() => roleMock.Object.Name.Should().Be("foo"));
        }

        [Specification]
        public void When_adding_users_to_roles()
        {
            "Given ".Context(() => SetupEmptyLists());
            "When ".Do(
                       () =>
                       _roleServiceExtended.AddUsersToRoles(new[] {userMock.Object, _userMock2.Object},
                                                            new[] {roleMock.Object, _roleMock2.Object}));
            "It should ".Assert(() =>
                                {
                                    roleMock.Object.Users.Should().Contain(userMock.Object);
                                    roleMock.Object.Users.Should().Contain(_userMock2.Object);
                                    _roleMock2.Object.Users.Should().Contain(userMock.Object);
                                    _roleMock2.Object.Users.Should().Contain(_userMock2.Object);
                                    userMock.Object.Roles.Should().Contain(roleMock.Object);
                                    userMock.Object.Roles.Should().Contain(_roleMock2.Object);
                                    _userMock2.Object.Roles.Should().Contain(roleMock.Object);
                                    _userMock2.Object.Roles.Should().Contain(_roleMock2.Object);
                                });
        }

        [Specification]
        public void When_removing_users_from_roles()
        {
            "Given ".Context(() =>
                             {
                                 SetupEmptyLists();
                                 userMock.Object.Roles = new List<IRoleExtended> {roleMock.Object, _roleMock2.Object};
                                 _userMock2.Object.Roles = new List<IRoleExtended> {roleMock.Object, _roleMock2.Object};
                                 roleMock.Object.Users = new List<IUserExtended> {userMock.Object, _userMock2.Object};
                                 _roleMock2.Object.Users = new List<IUserExtended> {userMock.Object, _userMock2.Object};
                             });
            "When ".Do(
                       () =>
                       _roleServiceExtended.RemoveUsersFromRoles(new[] {userMock.Object, _userMock2.Object},
                                                                 new[] {roleMock.Object, _roleMock2.Object}));
            "It should ".Assert(() =>
                                {
                                    roleMock.Object.Users.Should().BeEmpty();
                                    _roleMock2.Object.Users.Should().BeEmpty();
                                    userMock.Object.Roles.Should().BeEmpty();
                                    _userMock2.Object.Roles.Should().BeEmpty();
                                });
        }

        void SetupEmptyLists()
        {
            roleMock.SetupAllProperties();
            roleMock.SetupGet(r => r.Name).Returns(roleName);
            roleMock.SetupGet(r => r.Users).Returns(new List<IUserExtended>());
            _roleMock2.SetupAllProperties();
            _roleMock2.SetupGet(r => r.Name).Returns("user");
            _roleMock2.SetupGet(r => r.Users).Returns(new List<IUserExtended>());
            userMock.SetupAllProperties();
            userMock.SetupGet(u => u.UserName).Returns(userName);
            userMock.SetupGet(u => u.Roles).Returns(new List<IRoleExtended>());
            _userMock2.SetupAllProperties();
            _userMock2.SetupGet(u => u.UserName).Returns("someOtherUser");
            _userMock2.SetupGet(u => u.Roles).Returns(new List<IRoleExtended>());
        }

        void SetupRole()
        {
            roleMock.SetupAllProperties();
            roleMock.SetupGet(r => r.Name).Returns(roleName);
            roleMock.SetupGet(r => r.Users).Returns(new List<IUserExtended> {userMock.Object});
        }

        void SetupUser()
        {
            userMock.SetupAllProperties();
            userMock.SetupGet(u => u.UserName).Returns(userName);
            userMock.SetupGet(u => u.Roles).Returns(new List<IRoleExtended> {roleMock.Object});
        }
    }
}