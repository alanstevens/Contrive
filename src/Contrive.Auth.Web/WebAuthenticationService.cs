using System.Net;
using System.Security.Principal;
using System.Web.Security;

namespace Contrive.Auth.Web
{
    public class WebAuthenticationService : IAuthenticationService
    {
        public WebAuthenticationService(IFormsAuthService formsAuthService)
        {
            _formsAuthService = formsAuthService;
        }

        readonly IFormsAuthService _formsAuthService;

        public IPrincipal CurrentPrincipal { get { return HttpContextProvider.GetContext().User; } }

        public bool UserIsAuthenticated { get { return HttpContextProvider.GetContext().Request.IsAuthenticated; } }

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
            HttpContextProvider.GetContext().Response.SetStatus(HttpStatusCode.Unauthorized);
        }
    }
}