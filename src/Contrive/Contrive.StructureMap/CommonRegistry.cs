using System.Threading;
using Contrive.Common;
using StructureMap.Configuration.DSL;

namespace Contrive.StructureMap
{
    public class CommonRegistry : Registry
    {
        public CommonRegistry()
        {
            For<IEventAggregator>().Singleton().Use<EventAggregator>();
        }
    }
}