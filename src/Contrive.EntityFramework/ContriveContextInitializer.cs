using System.Data.Entity;
using System.Linq;
using Contrive.Core;
using Contrive.Core.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.EntityFramework
{
  public class ContriveContextInitializer : DropCreateDatabaseAlways<CointriveContext>
  {
    ContriveContextInitializer() { }

    public static void Initialize()
    {
      Database.SetInitializer(new ContriveContextInitializer());

      EnsureDbCreated();
    }

    static void EnsureDbCreated()
    {
      using (var db = new CointriveContext())
      {
        // This will create the database
        var test = db.Users.FirstOrDefault(usr => usr.UserName == "test");
      }
    }

    protected override void Seed(CointriveContext context)
    {
      var userService = ServiceLocator.Current.GetInstance<IUserService>();
      var roleService = ServiceLocator.Current.GetInstance<IRoleService>();
      var seedRoles = new[] {"Admin", "ProjectManager", "Developer",};

      seedRoles.Each(roleService.CreateRole);

      var seedUsers = new[] {new {Username = "test", Password = "test", Email = "test@test.com",}};

      seedUsers.Each(u => userService.CreateUser(u.Username, u.Password, u.Email));

      var userNames = seedUsers.Select(u => u.Username).ToArray();

      roleService.AddUsersToRoles(userNames, seedRoles);
    }
  }
}