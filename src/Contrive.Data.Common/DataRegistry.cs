using Contrive.Common;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace Contrive.Data.Common
{
  public class DataRegistry: Registry
  {
    public DataRegistry()
    {
      For<IUnitOfWork>().LifecycleIs(new ThreadLocalStorageLifecycle()).Use<UnitOfWork>();
    }
  }
}