namespace Contrive.Auth
{
    public interface IUserRepository
    {
        IUser GetUserByUserName(string userName);

        IUser GetUserByEmailAddress(string emailAddress);

        void Insert(IUser user);

        void Update(IUser user);

        void Delete(IUser user);
    }
}