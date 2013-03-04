
using Contrive.Common.Extensions;

namespace Contrive.Auth
{
    public class AuthenticationService : IAuthenticationService
    {
        public AuthenticationService(IUserService userService, IPlatformAuthenticationService platformAuthenticationService)
        {
            _userService = userService;
            _platformAuthenticationService = platformAuthenticationService;
        }

        readonly IPlatformAuthenticationService _platformAuthenticationService;
        readonly IUserService _userService;

        public bool LogOn(string userName, string password, bool rememberMe = false)
        {
            var user = _userService.ValidateUser(userName, password);
            if (user.IsNull()) return false;
            return _platformAuthenticationService.SignIn(user, rememberMe);
        }

        public void LogOff()
        {
            _platformAuthenticationService.SignOut();
        }
    }
}