using System.Data.Entity;
using System.Linq;
using Contrive.Core;
using Contrive.Core.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.EntityFramework
{
  public class ContextInitialization
  {
    public void Execute()
    {
      Database.SetInitializer(new EntityContextInitializer());

      EnsureDbCreated();
    }

    void EnsureDbCreated()
    {
      using (var db = new EntityContext())
      {
        //This will create the database
        // ReSharper disable ReturnValueOfPureMethodIsNotUsed
        var test = db.Users.FirstOrDefault(usr => usr.UserName == "test");
        // ReSharper restore ReturnValueOfPureMethodIsNotUsed
      }
    }
  }

  public class EntityContextInitializer : DropCreateDatabaseAlways<EntityContext>
  {
    protected override void Seed(EntityContext context)
    {
      var userManagementService = ServiceLocator.Current.GetInstance<IUserService>();
      var roleManagementService = ServiceLocator.Current.GetInstance<IRoleService>();
      var seedRoles = new[]
                  {
                    "Admin", 
                    "ProjectManager",
                    "Developer",
                  };

      seedRoles.Each(roleManagementService.CreateRole);

      var seedUsers = new[]
                  {
                    new 
                        {
                          Username = "test",
                          Password = "test",
                          Email = "test@test.com",
                          Roles = seedRoles
                        }
                  };

      seedUsers.Each(u => userManagementService.CreateUser(u.Username, u.Password, u.Email));

      var userNames = seedUsers.Select(u => u.Username).ToArray();

      roleManagementService.AddUsersToRoles(userNames, seedRoles);
    }
  }
}