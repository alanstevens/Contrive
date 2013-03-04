using System.Collections.Generic;

namespace Contrive.Auth
{
    public interface IUserRepository
    {
        IUser GetUserByUserName(string userName);

        IUser GetUserByEmailAddress(string emailAddress);

        void Insert(IUser user);

        void Update(IUser user);

        void Delete(IUser user);

        IEnumerable<IUser> GetUsersForUserNames(IEnumerable<string> userNames);
    }
}