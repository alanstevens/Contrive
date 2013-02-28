namespace Contrive.Auth
{
    public interface IUserServiceSettings
    {
        string Realm { get; set; }

        string ApplicationName { get; set; }

        bool EnablePasswordRetrieval { get; }

        bool EnablePasswordReset { get; }

        bool RequiresQuestionAndAnswer { get; }

        int MaxInvalidPasswordAttempts { get; }

        int PasswordAttemptWindow { get; }

        bool RequiresUniqueEmail { get; }

        UserPasswordFormat PasswordFormat { get; }

        int MinRequiredPasswordLength { get; }

        int MinRequiredNonAlphanumericCharacters { get; }

        string PasswordStrengthRegularExpression { get; }

        int MinPasswordLength { get; }

        string ContriveEmailFrom { get; }

        string ContriveEmailSubject { get; }

        string ContriveEmailTemplatePath { get; }
    }
}