using System.Data.Entity;
using System.Linq;
using Contrive.Auth.Membership;
using Contrive.Common.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Auth.EntityFramework.Membership
{
    public class MembershipContextInitializer : DropCreateDatabaseAlways<MembershipContext>
    {
        MembershipContextInitializer() {}

        public static void Initialize()
        {
            Database.SetInitializer(new MembershipContextInitializer());

            EnsureDbCreated();
        }

        static void EnsureDbCreated()
        {
            using (var db = new MembershipContext())
            {
                // This will create the database
#pragma warning disable 168
                var test = db.Users.FirstOrDefault(usr => usr.UserName == "test");
#pragma warning restore 168
            }
        }

        protected override void Seed(MembershipContext context)
        {
            var userService = ServiceLocator.Current.GetInstance<IUserServiceExtended>();
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