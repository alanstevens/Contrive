namespace Contrive.Auth
{
    public interface ILogOnService
    {
        bool LogOn(string userName, string password, bool rememberMe = false);

        void LogOff();
    }
}