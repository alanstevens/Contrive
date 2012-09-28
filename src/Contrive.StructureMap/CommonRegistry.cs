using System.Threading;
using Contrive.Common;
using StructureMap.Configuration.DSL;

namespace Contrive.StructureMap
{
  public class CommonRegistry: Registry
  {
    public CommonRegistry()
    {
      For<SynchronizationContext>().Singleton().Use(SynchronizationContext.Current);
      For<IEventAggregator>().Singleton().Use<EventAggregator>();
    }
  }
}