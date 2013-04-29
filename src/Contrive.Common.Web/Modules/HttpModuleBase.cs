using System;
using System.Web;

namespace Contrive.Common.Web.Modules
{
    public abstract class HttpModuleBase : DisposableBase, IHttpModule
    {
        protected Action OnBeginRequest = () => { };
        protected Action OnEndRequest = () => { };
        protected Action OnError = () => { };
        HttpApplication _application;

        protected HttpContextBase Context { get { return new HttpContextWrapper(_application.Context); } }

        public void Init(HttpApplication application)
        {
            _application = application;
            application.BeginRequest += (s, e) => OnBeginRequest();
            application.Error += (s, e) => OnError();
            application.EndRequest += (s, e) => OnEndRequest();
        }
    }
}