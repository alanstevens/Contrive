using System.Data.Entity;
using System.Linq;
using Contrive.Common.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Auth.EntityFramework
{
    public class ContriveContextInitializer : DropCreateDatabaseAlways<ContriveContext>
    {
        ContriveContextInitializer() {}

        public static void Initialize()
        {
            Database.SetInitializer(new ContriveContextInitializer());

            EnsureDbCreated();
        }

        static void EnsureDbCreated()
        {
            using (var db = new ContriveContext())
            {
                // This will create the database
#pragma warning disable 168
                var test = db.Users.FirstOrDefault(usr => usr.UserName == "test");
#pragma warning restore 168
            }
        }

        protected override void Seed(ContriveContext context)
        {
            var userService = ServiceLocator.Current.GetInstance<IUserService>();

            var seedUsers = new[] {new {Username = "test", Password = "test", Email = "test@test.com",}};

            seedUsers.Each(u => userService.CreateUser(u.Username, u.Password, u.Email));
        }
    }
}