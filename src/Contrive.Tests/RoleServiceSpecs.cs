using System.Collections.Generic;
using System.Collections.Specialized;
using Contrive.Core;
using FluentAssertions;
using Moq;
using SubSpec;

namespace Contrive.Tests
{
  public class Using_RoleServiceSpecs
  {
    readonly Mock<IConfigurationProvider> _configuration = new Mock<IConfigurationProvider>();
    readonly Mock<IRoleRepository> _roleRepository = new Mock<IRoleRepository>();
    readonly Mock<IUserRepository> _userRepository = new Mock<IUserRepository>();
    RoleService _roleService;

    [Specification]
    public void Get_all_should_not_be_empty()
    {
      "Given a new RoleService"
        .Context(() =>
        {
          _roleRepository.Setup(r => r.GetAll()).Returns(new List<IRole> {new Mock<IRole>().Object});
          _configuration.SetupGet(c => c.RoleManagerConfiguration).Returns(new NameValueCollection());
          _roleService = new RoleService(_roleRepository.Object, _userRepository.Object, _configuration.Object);
        });
      "Get all should not be empty".Assert(() => _roleService.GetAllRoles().Should().NotBeEmpty());
    }
  }
}