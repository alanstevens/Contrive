using System;

namespace Contrive.Auth
{
    public interface IAuthenticationService
    {
        bool LogOn(string userName, string password, bool rememberMe = false);

        void LogOff();

        void SignOut(string userName);

        void SignOut(Guid userId);
    }
}