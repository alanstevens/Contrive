using Contrive.Auth;
using Contrive.Auth.Web;
using Contrive.Common;
using Contrive.Common.Web;
using Contrive.Common.Web.Errors;
using StructureMap.Configuration.DSL;

namespace Contrive.StructureMap
{
    public class WebCommonRegistry : Registry
    {
        public WebCommonRegistry()
        {
            For<IWebConfigurationProvider>()
                .Singleton()
                .Use<WebConfigurationProvider>();
            For<IErrorViewRenderer>()
                .Singleton()
                .Use<MvcErrorViewRenderer>();
            For<ICache>()
                .HttpContextScoped()
                .Use<Cache>();
            For<IAuthenticationService>()
                .HttpContextScoped()
                .Use<WebAuthenticationService>();
            For<IFormsAuthService>()
                .HttpContextScoped()
                .Use<FormsAuthService>();
        }
    }
}