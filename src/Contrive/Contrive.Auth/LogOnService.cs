using Contrive.Common.Extensions;

namespace Contrive.Auth
{
    public class LogOnService : ILogOnService
    {
        public LogOnService(IUserService userService, IAuthenticationService authenticationService)
        {
            _userService = userService;
            _authenticationService = authenticationService;
        }

        readonly IAuthenticationService _authenticationService;
        readonly IUserService _userService;

        public bool LogOn(string userName, string password, bool rememberMe = false)
        {
            var user = _userService.ValidateUser(userName, password);
            if (user.IsNull()) return false;

            if (_authenticationService.SignIn(user, rememberMe))
            {
                _userService.CurrentUser = user;
                return true;
            }
            return false;
        }

        public void LogOff()
        {
            _authenticationService.SignOut();
        }
    }
}