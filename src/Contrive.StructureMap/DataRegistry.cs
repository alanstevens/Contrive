using Contrive.Common;
using Contrive.Data.Common;
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