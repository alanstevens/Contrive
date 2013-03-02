using System;
using System.Web;
using Contrive.Common;

namespace Contrive.Web.Common.Modules
{
    public abstract class HttpModuleBase : DisposableBase, IHttpModule
    {
        protected Action OnBeginRequest = () => { };
        protected Action OnEndRequest = () => { };
        protected Action OnError = () => { };
        protected HttpContextBase _context;

        public void Init(HttpApplication application)
        {
            _context = new HttpContextWrapper(application.Context);

            application.BeginRequest += (s, e) => OnBeginRequest();
            application.Error += (s, e) => OnError();
            application.EndRequest += (s, e) => OnEndRequest();
        }
    }
}