using System.Web;
using Contrive.Web.Common.Modules;
using StructureMap;

namespace Contrive.StructureMap
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