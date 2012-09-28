using System;
using Contrive.Common.Extensions;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace Contrive.Common
{
  /// <summary>
  ///   Insures that types implementing IListener are registered with the container as singletons.
  /// </summary>
  public class ListenerConvention: IRegistrationConvention
  {
    public void Process(Type type, Registry registry)
    {
      Type interfaceType = null;

      if (!type.TryGetInterface<IListener>(out interfaceType)) return;

      registry.For(interfaceType).Singleton().Use(type);
    }
  }
}