using System.Web;

namespace Contrive.Auth.Web.Modules
{
    // Implementation derived from: http://www.rassoc.com/gregr/weblog/2002/07/09/web-services-security-http-digest-authentication-without-active-directory/

    // Make the following changes to your web.config file (within <system.web>):
    //     - change authentication line to: <authentication mode="None" /> 
    //     - add an authorization section if you wish, such as
    //         <authorization>
    //           <deny users="?" />
    //         </authorization>
    //     - add the following lines:
    //         <httpModules>
    //           <add name="DigestAuthenticationModule" 
    //                type="Rassoc.Samples.DigestAuthenticationModule,DigestAuthMod" />
    //         </httpModules>   
    // Add the following to your web.config (within <configuration>):
    //         <appSettings>
    //           <add key="Rassoc.Samples.DigestAuthenticationModule_Realm" value="RassocDigestSample" />
    //           <add key="Rassoc.Samples.DigestAuthenticationModule_UserFileVpath" value="~/users.xml" />
    //         </appSettings>
    public class DigestAuthenticationModule : AuthenticationModuleBase
    {
        protected override bool Authenticate(HttpApplication app)
        {
            return DigestHelper.Authenticate(app);
        }

        protected override string BuildChallengeHeader(HttpApplication app)
        {
            return DigestHelper.BuildChallengeHeader(app);
        }
    }
}