using System;
using System.Web;
using Contrive.Common;

namespace Contrive.Web.Common
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