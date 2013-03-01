namespace Contrive.Auth.Membership
{
    public interface IMembershipSettings : IUserServiceSettings
    {
        string ApplicationName { get; }

        bool EnablePasswordRetrieval { get; }

        bool EnablePasswordReset { get; }

        bool RequiresQuestionAndAnswer { get; }

        int MaxInvalidPasswordAttempts { get; }

        int PasswordAttemptWindow { get; }
    }
}