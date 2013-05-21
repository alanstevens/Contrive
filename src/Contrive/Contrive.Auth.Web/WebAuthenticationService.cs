using System.Web.Security;
using Contrive.Common.Web;

namespace Contrive.Auth.Web
{
    public class WebAuthenticationService : IAuthenticationService
    {
        public WebAuthenticationService(IFormsAuthService formsAuthService)
        {
            _formsAuthService = formsAuthService;
        }

        readonly IFormsAuthService _formsAuthService;

        /// <summary>
        ///     User is already authenticated. We are telling forms authentication who he is.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="rememberMe"></param>
        /// <returns></returns>
        public bool SignIn(IUser user, bool rememberMe = false)
        {
            var context = HttpContextProvider.GetContext();
            var principal = _formsAuthService.GetUpdatedPrincipalFor(user, rememberMe);

            context.User = principal;

            return true;
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}