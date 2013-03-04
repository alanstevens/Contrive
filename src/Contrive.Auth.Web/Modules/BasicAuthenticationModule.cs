using System;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Web;
using Contrive.Common;
using Contrive.Common.Extensions;

namespace Contrive.Auth.Web.Modules
{
    // These constants are pretty much all given by RFC2616 and RFC2617
    public class BasicAuthenticationModule : AuthenticationModuleBase
    {
        public BasicAuthenticationModule()
        {
            _realm = GetConfigurationProvider().AppSettings["HTTPAuth.Components.AuthBasic_Realm"];
        }

        const string AUTHENTICATION_METHOD_NAME = "Basic";
        const string CHALLENGE_HEADER_VALUE = "Basic realm=\"{0}\"";

        protected override bool Authenticate(HttpApplication app)
        {
            // Get authentication data
            var authString = app.Request.Headers[RESPONSE_HEADER_NAME];

            if (String.IsNullOrEmpty(authString)) return true;

            if (!authString.StartsWith(AUTHENTICATION_METHOD_NAME, StringComparison.OrdinalIgnoreCase)) return true;

            // Get username and password
            string userName, password;

            ParseUserNameAndPassword(authString, out userName, out password);

            // Validate user
            if (GetUserService().ValidateUser(userName, password).IsNotNull())
            {
                // Success - set user
                var identity = new GenericIdentity(userName, AUTHENTICATION_METHOD_NAME);
                app.Context.User = new GenericPrincipal(identity, new string[0]);
                return true;
            }
            return false;
        }

        protected override string BuildChallengeHeader(HttpApplication app)
        {
            return string.Format(CHALLENGE_HEADER_VALUE, _realm);
        }

        static void ParseUserNameAndPassword(string authString, out string userName, out string password)
        {
            try
            {
                authString = Encoding.UTF8.GetString(Convert.FromBase64String(authString.Substring(6)));
                var authParts = authString.Split(new[] {':'}, 2);
                userName = authParts[0];
                password = authParts[1];
            }
            catch (Exception ex)
            {
                throw new SecurityException("Invalid format of '" + RESPONSE_HEADER_NAME + "' HTTP header.", ex);
            }
        }
    }
}