using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Contrive.Auth;
using Contrive.Common.Extensions;
using Contrive.Common.Data;
using Contrive.Sample.Data.Properties;

namespace Contrive.Sample.Data
{
    public class UserRepository : DataServiceBase, IUserRepository
    {
        const string USERNAME_PREDICATE = " WHERE [FirstName]+[LastName]=";
        const string EMAIL_PREDICATE = " WHERE [EmailAddress]=";

        public IUser GetUserByUserName(string userName)
        {
            return GetSingleUser(userName, USERNAME_PREDICATE);
        }

        public IUser GetUserByEmailAddress(string emailAddress)
        {
            return GetSingleUser(emailAddress, EMAIL_PREDICATE);
        }

        public void Insert(IUser user) {}

        public void Update(IUser user)
        {
            var query = @"UPDATE [AdventureWorksLT2008R2].[SalesLT].[Customer]
                                        Set
                                           [FirstName]='{0}'
                                          ,[LastName]='{1}'
                                          ,[EmailAddress]='{2}'
                                          ,[PasswordHash]='{3}'
                                          ,[PasswordSalt]='{4}' 
                                        WHERE [FirstName]+[LastName]='{5}';".FormatWith(user.FirstName.Replace("'","''"), user.LastName.Replace("'","''"), user.Email, user.PasswordHash, user.PasswordSalt,user.FirstName.Replace("'","''")+user.LastName.Replace("'","''"));
            var cmd = GetCommand();
            cmd.SetCommandText(query);
            var count = cmd.ExecuteNonQuery();
            if (count < 1) Debugger.Break();
        }

        public void Delete(IUser user) {}

        public IEnumerable<IUser> GetUsersForUserNames(IEnumerable<string> userNames)
        {
            var predicate = "where ";

            var query = "{0} {1}".FormatWith(Resources.UserQuery, predicate);

            var cmd = GetCommand()
                .SetCommandText(query);

            var rows = cmd.ExecuteEnumerable();

            if (rows == null || !rows.Any()) return new IUser[0];

            var users = rows.Select(BuildUser)
                            .ToList();

            return users;
        }

        public IEnumerable<IUser> GetAll()
        {
            var cmd = GetCommand()
                .SetCommandText(Resources.UserQuery);

            var rows = cmd.ExecuteEnumerable();

            if (rows == null || !rows.Any()) return new IUser[0];

            var users = rows.Select(BuildUser)
                            .ToList();

            return users;
        }

        static User BuildUser(DataRow r)
        {
            var firstName = r.GetString("FirstName");
            var lastName = r.GetString("LastName");

            return new User {
                Id = r.GetInt("CustomerID"),
                UserName = "{0}{1}".FormatWith(firstName, lastName),
                FirstName = firstName,
                LastName = lastName,
                Email = r.GetString("EmailAddress"),
                PasswordHash = r.GetString("PasswordHash"),
                PasswordSalt = r.GetString("PasswordSalt")
            };
        }

        IUser GetSingleUser(string userNameOrEmail, string predicate)
        {
            var query = "{0} {1}@UserNameOrEmail".FormatWith(Resources.UserQuery, predicate);

            var cmd = GetCommand()
                .SetCommandText(query);

            cmd.AddParameter("@UserNameOrEmail", DbType.String, userNameOrEmail);

            var row = cmd.ExecuteSingle();

            return row.IsNull()
                       ? null
                       : BuildUser(row);
        }
    }
}