using Contrive.Common;
using Contrive.Common.Data;
using StructureMap.Configuration.DSL;

namespace Contrive.StructureMap
{
    public class DataRegistry : Registry
    {
        public DataRegistry()
        {
            For<IUnitOfWork>().HybridHttpOrThreadLocalScoped().Use<UnitOfWork>();
        }
    }
}