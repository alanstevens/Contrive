using System.Web;
using Contrive.Common;

namespace Contrive.Web.Common.Modules
{
  public abstract class BaseHttpModule : DisposableBase, IHttpModule
  {
    public void Init(HttpApplication application)
    {
      var context = new HttpContextWrapper(application.Context);

      application.BeginRequest += (s, e) => OnBeginRequest(context);
      application.Error += (s, e) => OnError(context);
      application.EndRequest += (s, e) => OnEndRequest(context);
    }

    public virtual void OnBeginRequest(HttpContextBase context) {}

    public virtual void OnError(HttpContextBase context) {}

    public virtual void OnEndRequest(HttpContextBase context) {}
  }
}