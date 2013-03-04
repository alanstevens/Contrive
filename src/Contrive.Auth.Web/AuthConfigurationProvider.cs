using System.Collections.Specialized;
using System.Diagnostics;
using System.Web.Configuration;
using Contrive.Common;

namespace Contrive.Auth.Web
{
    public class AuthConfigurationProvider : ConfigurationProvider, IAuthConfigurationProvider
    {
        public NameValueCollection UserServiceConfiguration
        {
            [DebuggerStepThrough] get { return GetUserServiceSettings(); }
        }

        public NameValueCollection RoleServiceConfiguration { get; private set; }

        NameValueCollection GetUserServiceSettings()
        {
            // TODO: HAS 02/28/2013 Make this a base class for both client and web versions.
            var settings = new NameValueCollection
            {
                {"HTTP.Realm", AppSettings["HTTP.Realm"]},
                {"ContriveEmailFrom", AppSettings["EmailSender"]},
                {"ContriveEmailSubject", AppSettings["EmailSubject"]},
                {"ContriveEmailTemplatePath", AppSettings["EmailTemplatePath"]},
                {"MaxHashedPasswordLength", AppSettings["MaxHashedPasswordLength"]}
            };

            var membershipSection = GetSection<MembershipSection>("system.web/membership");
            var defaultProvider = membershipSection.DefaultProvider;
            var membershipSettings = membershipSection.Providers[defaultProvider].Parameters;
            // Values used from Membership Settings:
            //RequiresUniqueEmail
            //PasswordFormat
            //MinRequiredPasswordLength
            //MinRequiredNonAlphanumericCharacters 
            //PasswordStrengthRegularExpression
            settings.Add(membershipSettings);
            return settings;
        }
    }
}