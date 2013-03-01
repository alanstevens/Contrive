using System;
using System.Collections.Specialized;
using Contrive.Common;

namespace Contrive.Auth.Membership
{
    public class MembershipSettings : UserServiceSettings, IMembershipSettings
    {
        public MembershipSettings(NameValueCollection settings)
            : base(settings)
        {
            Verify.NotNull(settings, "settings");

            // Membership Settings
            ApplicationName = GetConfigValue(settings["applicationName"], "/");

            EnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(settings["enablePasswordReset"], "false"));

            EnablePasswordReset = Convert.ToBoolean(GetConfigValue(settings["enablePasswordRetrieval"], "true"));

            RequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(settings["requiresQuestionAndAnswer"], "false"));

            MaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(settings["maxInvalidPasswordAttempts"], "5"));

            PasswordAttemptWindow = Convert.ToInt32(GetConfigValue(settings["passwordAttemptWindow"], "10"));
        }

        public string ApplicationName { get; private set; }

        public bool EnablePasswordRetrieval { get; private set; }

        public bool EnablePasswordReset { get; private set; }

        public bool RequiresQuestionAndAnswer { get; private set; }

        public int MaxInvalidPasswordAttempts { get; private set; }

        public int PasswordAttemptWindow { get; private set; }
    }
}