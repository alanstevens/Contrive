using Contrive.Common;
using Contrive.Data.Common;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace Contrive.StructureMap
{
  public class DataRegistry : Registry
  {
    public DataRegistry()
    {
      For<IUnitOfWork>().LifecycleIs(new ThreadLocalStorageLifecycle()).Use<UnitOfWork>();
    }
  }
}