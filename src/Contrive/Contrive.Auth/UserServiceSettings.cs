using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using Contrive.Common;
using Contrive.Common.Extensions;

namespace Contrive.Auth
{
    public class UserServiceSettings : IUserServiceSettings
    {
        public UserServiceSettings(NameValueCollection settings)
        {
            Verify.NotNull(settings, "settings");

            Realm = GetConfigValue(settings["HTTP.Realm"], "Application");

            EmailSender = GetConfigValue(settings["EmailSender"], "Application");

            EmailSubject = GetConfigValue(settings["EmailSubject"], "Password Reset Request");

            EmailTemplatePath = GetConfigValue(settings["EmailTemplatePath"],
                                               "~/Content/Contrive/ResetPassword.html");

            MaxHashedPasswordLength = Convert.ToInt32(GetConfigValue(settings["MaxHashedPasswordLength"], "128"));

            RequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(settings["requiresUniqueEmail"], "true"));

            MinRequiredNonAlphanumericCharacters =
                Convert.ToInt32(GetConfigValue(settings["minRequiredNonAlphanumericCharacters"], "0"));

            MinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(settings["minRequiredPasswordLength"], "6"));

            PasswordStrengthRegularExpression = GetConfigValue(settings["passwordStrengthRegularExpression"], "");

            var format = settings["passwordFormat"] ?? "Hashed";

            switch (format)
            {
                case "Hashed":
                    PasswordFormat = UserPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    PasswordFormat = UserPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    throw new ArgumentException("Clear text password storage is not allowed.");
                default:
                    throw new ProviderException("Password format not supported.");
            }
        }

        public string Realm { get; private set; }

        public string EmailSender { get; private set; }

        public string EmailSubject { get; private set; }

        public string EmailTemplatePath { get; private set; }

        public int MaxHashedPasswordLength { get; private set; }

        public bool RequiresUniqueEmail { get; private set; }

        public UserPasswordFormat PasswordFormat { get; private set; }

        public int MinRequiredPasswordLength { get; private set; }

        public int MinRequiredNonAlphanumericCharacters { get; private set; }

        public string PasswordStrengthRegularExpression { get; private set; }

        protected static string GetConfigValue(string configValue, string defaultValue)
        {
            return configValue.IsEmpty() ? defaultValue : configValue;
        }
    }
}