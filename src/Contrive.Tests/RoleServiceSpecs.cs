using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Contrive.Core;
using FluentAssertions;
using Microsoft.Practices.ServiceLocation;
using Moq;
using SubSpec;

namespace Contrive.Tests
{
  public class Using_RoleServiceSpecs
  {
    public Using_RoleServiceSpecs()
    {
      _configuration.SetupGet(c => c.RoleManagerConfiguration).Returns(new NameValueCollection());
      _roleService = new RoleService(_roleRepository.Object, _userRepository.Object, _configuration.Object);
    }

    readonly Mock<IConfigurationProvider> _configuration = new Mock<IConfigurationProvider>();
    readonly Mock<IRoleRepository> _roleRepository = new Mock<IRoleRepository>();
    readonly RoleService _roleService;
    readonly Mock<IUserRepository> _userRepository = new Mock<IUserRepository>();
    readonly Mock<IRole> roleMock = new Mock<IRole>();
    readonly Mock<IUser> userMock = new Mock<IUser>();
    string roleName = "admin";
    string userName = "someUser";
    readonly Mock<IRole> _roleMock2 = new Mock<IRole>();
    readonly Mock<IUser> _userMock2 = new Mock<IUser>();

    [Specification]
    public void Get_all_should_not_be_empty()
    {
      "Given a new RoleService"
        .Context(() => _roleRepository.Setup(r => r.GetAll()).Returns(new List<IRole> { new Mock<IRole>().Object }));
      "Get all should not be empty".Assert(() => _roleService.GetAllRoles().Should().NotBeEmpty());
    }

    [Specification]
    public void When_checing_users_in_a_role()
    {
      bool userInRole = false;
      "Given ".Context(() =>
      {
        SetupUser();
        SetupRole();
        _roleRepository.Setup(rr => rr.FirstOrDefault(It.IsAny<Func<IRole, bool>>()))
          .Returns(roleMock.Object);
        _userRepository.Setup(ur => ur.FirstOrDefault(It.IsAny<Func<IUser, bool>>()))
          .Returns(userMock.Object);
      });
      "When ".Do(() => userInRole = _roleService.IsUserInRole(userName, roleName));
      "It should ".Assert(() => { userInRole.Should().BeTrue(); });
    }

    [Specification]
    public void When_checking_if_role_exists()
    {
      bool roleExists = false;
      "Given ".Context(() =>
      {
        SetupRole();
        _roleRepository.Setup(rr => rr.FirstOrDefault(It.IsAny<Func<IRole, bool>>()))
          .Returns(roleMock.Object);
      });
      "When ".Do(() => roleExists = _roleService.RoleExists(roleName));
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
        _roleRepository.Setup(rr => rr.FirstOrDefault(It.IsAny<Func<IRole, bool>>()))
          .Returns(roleMock.Object);
        _userRepository.Setup(ur => ur.FirstOrDefault(It.IsAny<Func<IUser, bool>>()))
          .Returns(userMock.Object);
      });
      "When ".Do(() => userIsInRole = _roleService.IsUserInRole(userName, roleName));
      "It should ".Assert(() => userIsInRole.Should().BeTrue());
    }

    [Specification]
    public void When_finding_roles_matching_a_username_pattern()
    {
      SetupUser();
      SetupRole();
      IEnumerable<IUser> usersInRole = null;
      IQueryable<IRole> query = new EnumerableQuery<IRole>(new[] { roleMock.Object });
      "Given ".Context(() => _roleRepository.Setup(rr => rr.GetQuery()).Returns(query));
      "When ".Do(() => usersInRole = _roleService.FindUsersInRole(roleName, userName.Substring(0, 4)));
      "It should ".Assert(() => usersInRole.Contains(userMock.Object));
      "It should ".Assert(() => usersInRole.Count().Should().Be(1));
    }

    [Specification]
    public void When_deleting_a_role()
    {
      SetupUser();
      SetupRole();
      "Given ".Context(() => _roleRepository.Setup(rr => rr.FirstOrDefault(It.IsAny<Func<IRole, bool>>()))
                               .Returns(roleMock.Object));
      "When ".Do(() => _roleService.DeleteRole(roleName, false));
      "It should ".Assert(() => userMock.Object.Roles.Should().NotContain(roleMock.Object));
    }

    [Specification]
    public void When_getting_roles_for_a_user()
    {
      SetupUser();
      IEnumerable<IRole> roles = null;
      "Given ".Context(() => _userRepository.Setup(ur => ur.FirstOrDefault(It.IsAny<Func<IUser, bool>>()))
                               .Returns(userMock.Object));
      "When ".Do(() => roles = _roleService.GetRolesForUser(userName));
      "It should ".Assert(() => roles.Should().Contain(roleMock.Object));
      "It should ".Assert(() => roles.Count().Should().Be(1));
    }

    [Specification]
    public void When_creating_a_role()
    {
      "Given ".Context(() =>
      {
        var serviceLocator = new Mock<IServiceLocator>();
        serviceLocator.Setup(sl => sl.GetInstance<IRole>()).Returns(roleMock.Object);
        ServiceLocator.SetLocatorProvider(() => serviceLocator.Object);
        roleMock.SetupProperty(r => r.Name);
      });
      "When ".Do(() => _roleService.CreateRole("foo"));
      "It should ".Assert(() => roleMock.Object.Name.Should().Be("foo"));
    }

    [Specification]
    public void When_adding_users_to_roles()
    {
      "Given ".Context(() => SetupEmptyLists());
      "When ".Do(() => _roleService.AddUsersToRoles(new[] { userMock.Object, _userMock2.Object }, new[] { roleMock.Object, _roleMock2.Object }));
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
        userMock.Object.Roles = new List<IRole> { roleMock.Object, _roleMock2.Object };
        _userMock2.Object.Roles = new List<IRole> { roleMock.Object, _roleMock2.Object };
        roleMock.Object.Users = new List<IUser> { userMock.Object, _userMock2.Object };
        _roleMock2.Object.Users = new List<IUser> { userMock.Object, _userMock2.Object };
      });
      "When ".Do(() => _roleService.RemoveUsersFromRoles(new[] { userMock.Object, _userMock2.Object }, new[] { roleMock.Object, _roleMock2.Object }));
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
      roleMock.SetupGet(r => r.Users).Returns(new List<IUser>());
      _roleMock2.SetupAllProperties();
      _roleMock2.SetupGet(r => r.Name).Returns("user");
      _roleMock2.SetupGet(r => r.Users).Returns(new List<IUser>());
      userMock.SetupAllProperties();
      userMock.SetupGet(u => u.Username).Returns(userName);
      userMock.SetupGet(u => u.Roles).Returns(new List<IRole>());
      _userMock2.SetupAllProperties();
      _userMock2.SetupGet(u => u.Username).Returns("someOtherUser");
      _userMock2.SetupGet(u => u.Roles).Returns(new List<IRole>());
    }

    void SetupRole()
    {
      roleMock.SetupAllProperties();
      roleMock.SetupGet(r => r.Name).Returns(roleName);
      roleMock.SetupGet(r => r.Users).Returns(new List<IUser> { userMock.Object });
    }

    void SetupUser()
    {
      userMock.SetupAllProperties();
      userMock.SetupGet(u => u.Username).Returns(userName);
      userMock.SetupGet(u => u.Roles).Returns(new List<IRole> { roleMock.Object });
    }
  }
}