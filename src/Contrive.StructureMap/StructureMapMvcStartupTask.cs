using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using StructureMap;

namespace Contrive.StructureMap
{
  public class StructureMapMvcStartupTask
  {
    public StructureMapMvcStartupTask()
    {
        var container = ServiceLocator.Current.GetInstance<IContainer>();
        var dependencyResolver = new StructureMapDependencyResolver(container);
        DependencyResolver.SetResolver(dependencyResolver);
    }
  }
}