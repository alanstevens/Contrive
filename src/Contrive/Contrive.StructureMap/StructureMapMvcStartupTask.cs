using System.Web.Mvc;
using Contrive.Common;
using Microsoft.Practices.ServiceLocation;
using StructureMap;

namespace Contrive.StructureMap
{
    public class StructureMapMvcStartupTask : IStartupTask
    {
        public void OnStartup()
        {
            var container = ServiceLocator.Current.GetInstance<IContainer>();
            var dependencyResolver = new StructureMapDependencyResolver(container);
            DependencyResolver.SetResolver(dependencyResolver);
        }
    }
}