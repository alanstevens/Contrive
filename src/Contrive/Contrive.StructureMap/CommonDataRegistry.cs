using Contrive.Common;
using Contrive.Common.Data;
using StructureMap.Configuration.DSL;

namespace Contrive.StructureMap
{
    public class CommonDataRegistry : Registry
    {
        public CommonDataRegistry()
        {
            For<IEventAggregator>().Singleton().Use<EventAggregator>();
            For<UnitOfWork>().HybridHttpOrThreadLocalScoped();
        }
    }
}