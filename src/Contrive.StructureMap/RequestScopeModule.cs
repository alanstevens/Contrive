using StructureMap;

namespace Contrive.StructureMap
{
    public class RequestScopeModule : HttpModuleBase
    {
        public RequestScopeModule()
        {
            OnEndRequest = ObjectFactory.ReleaseAndDisposeAllHttpScopedObjects;
        }

        protected override void OnDisposing(bool disposing) {}
    }
}