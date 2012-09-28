using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using StructureMap;

namespace Contrive.StructureMap
{
  public class StructureMapMvcStartupTask
  {
    public StructureMapMvcStartupTask()
    {
      DependencyResolver.SetResolver(new StructureMapDependencyResolver(ServiceLocator.Current.GetInstance<IContainer>()));
    }
  }
}