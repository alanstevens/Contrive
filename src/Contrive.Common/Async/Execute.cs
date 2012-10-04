using System;
using System.Threading;

namespace Contrive.Common.Async
{
  public class Execute : IStartupTask
  {
    public static Action<Action, SynchronizationContext> Executor = (action, synchronizationContext) => action();

    public void OnStartup()
    {
      Executor = (action, synchronizationContext) =>
                 synchronizationContext.Post(state =>
                                             {
                                               action();
                                               synchronizationContext.OperationCompleted();
                                             }, null);
    }

    public static void OnMainThread(Action action, SynchronizationContext synchronizationContext)
    {
      Executor.Invoke(action, synchronizationContext);
    }
  }
}