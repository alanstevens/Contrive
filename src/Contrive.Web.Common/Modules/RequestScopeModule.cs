using System.Web;
using StructureMap;

namespace Contrive.Web.Common.Modules
{
  public class RequestScopeModule : BaseHttpModule
  {
    public override void OnBeginRequest(HttpContextBase context) { }

    public override void OnEndRequest(HttpContextBase context)
    {
      ObjectFactory.ReleaseAndDisposeAllHttpScopedObjects();
    }

    protected override void OnDisposing(bool disposing) { }
  }
}