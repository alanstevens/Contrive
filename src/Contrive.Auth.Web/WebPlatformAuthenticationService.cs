using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using Contrive.Web.Common;

namespace Contrive.Auth.Web
{
    public class WebPlatformAuthenticationService : IPlatformAuthenticationService
    {
        readonly IFormsAuthService _formsAuthService;

        public WebPlatformAuthenticationService(IFormsAuthService formsAuthService)
        {
            _formsAuthService = formsAuthService;
        }

        static HttpContextBase Context { get { return new HttpContextWrapper(HttpContext.Current); } }

        public IPrincipal CurrentPrincipal { get { return Context.User; } }

        public bool UserIsAuthenticated { get { return Context.Request.IsAuthenticated; } }

        public bool SignIn(IUser user, bool rememberMe = false)
        {
            _formsAuthService.UpdateCurrentUserWith(user, rememberMe);
            return true;
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }

        public void Deauthorize()
        {
            Context.Response.SetStatus(HttpStatusCode.Unauthorized);
        }
    }
}