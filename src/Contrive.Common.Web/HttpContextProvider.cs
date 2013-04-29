using System;
using System.Web;

namespace Contrive.Common.Web
{
    public class HttpContextProvider : IStartupTask
    {
        public static Func<HttpContextBase> GetContext = () => null;

        public void OnStartup()
        {
            GetContext = () => new HttpContextWrapper(HttpContext.Current);
        }
    }
}