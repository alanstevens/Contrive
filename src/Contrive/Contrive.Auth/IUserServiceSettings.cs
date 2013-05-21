namespace Contrive.Auth
{
    public interface IUserServiceSettings
    {
        string Realm { get; }

        string EmailSender { get; }

        string EmailSubject { get; }

        string EmailTemplatePath { get; }

        int MaxHashedPasswordLength { get; }

        bool RequiresUniqueEmail { get; }

        UserPasswordFormat PasswordFormat { get; }

        int MinRequiredPasswordLength { get; }

        int MinRequiredNonAlphanumericCharacters { get; }

        string PasswordStrengthRegularExpression { get; }
    }
}