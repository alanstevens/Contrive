using System;
using StructureMap;
using StructureMap.Interceptors;
using StructureMap.TypeRules;

namespace Contrive.Common
{
  /// <summary>
  ///   Insures that when types implementing IHandle<E> are instantiated, they are subscribed to the appropriate event type with the event aggregator
  /// </summary>
  public class SubscriptionInterceptor: TypeInterceptor
  {
    public object Process(object target, IContext context)
    {
      context.GetInstance<IEventAggregator>().Subscribe(target);

      return target;
    }

    public bool MatchesType(Type type)
    {
      return type.ImplementsInterfaceTemplate(typeof(IHandle<>));
    }
  }
}