using System.Collections.Specialized;
using System.Diagnostics;
using System.Web.Configuration;
using Contrive.Auth.Membership;
using Contrive.Common;

namespace Contrive.Auth.Web.Membership
{
    public class MembershipConfigurationProvider : ConfigurationProvider, IMembershipConfigurationProvider
    {
        public NameValueCollection UserServiceConfiguration
        {
            [DebuggerStepThrough] get { return GetMembershipSettings(); }
        }

        public NameValueCollection RoleServiceConfiguration
        {
            [DebuggerStepThrough]
            get
            {
                var roleManagerSection = GetSection<RoleManagerSection>("system.web/roleManager");
                var defaultProvider = roleManagerSection.DefaultProvider;
                return roleManagerSection.Providers[defaultProvider].Parameters;
            }
        }

        NameValueCollection GetMembershipSettings()
        {
            var membershipSection = GetSection<MembershipSection>("system.web/membership");
            var defaultProvider = membershipSection.DefaultProvider;
            var settings = membershipSection.Providers[defaultProvider].Parameters;
            settings.Add("HTTP.Realm", AppSettings["HTTP.Realm"]);
            settings.Add("ContriveEmailFrom", AppSettings["ContriveEmailFrom"]);
            settings.Add("ContriveEmailSubject", AppSettings["ContriveEmailSubject"]);
            settings.Add("ContriveEmailTemplatePath", AppSettings["ContriveEmailTemplatePath"]);
            return settings;
        }
    }
}