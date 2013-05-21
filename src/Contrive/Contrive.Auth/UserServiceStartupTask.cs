using Contrive.Common;

namespace Contrive.Auth
{
    public class UserServiceStartupTask : IStartupTask
    {
        public void OnStartup()
        {
            UserService.NewUser = () => new User();
        }
    }
}