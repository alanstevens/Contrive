using System.Collections.Generic;

namespace Contrive.Auth.Membership
{
    public interface IUserExtendedRepository
    {
        IUserExtended GetUserByUserName(string userName);

        IUserExtended GetUserByEmailAddress(string emailAddress);

        void Insert(IUserExtended user);

        void Update(IUserExtended user);

        void Delete(IUserExtended user);

        IEnumerable<IUserExtended> GetAll();

        IEnumerable<IUserExtended> FindUsersForUserName(string searchTerm);

        IEnumerable<IUserExtended> FindUsersForEmailAddress(string searchTerm);

        IEnumerable<IUserExtended> GetUsersForUserNames(IEnumerable<string> userNames);

        IUserExtended GetUserByConfirmationToken(string token);

        IUserExtended GetUserByPasswordVerificationToken(string token);
    }
}