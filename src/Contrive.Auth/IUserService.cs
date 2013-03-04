namespace Contrive.Auth
{
    public interface IUserService
    {
        bool ChangePassword(string userName, string oldPassword, string newPassword);

        IUser GetUserByUserName(string userName);

        IUser ValidateUser(string userName, string password);

        UserCreateStatus CreateUser(string userName,
                                    string password,
                                    string emailAddress,
                                    string firstName = null,
                                    string lastName = null);

        bool DeleteAccount(string userName);

        IUser GetUserByEmailAddress(string emailAddress);

        void UpdateUser(IUser user);
    }
}