using System;
using System.Threading;
using System.Threading.Tasks;
using Contrive.Common.Extensions;

namespace Contrive.Common.Async
{
  public class Execute : IStartupTask
  {
    readonly SynchronizationContext _sc;
    public static Action<Action> Executor = action => action();
    TaskScheduler _taskScheduler;

    public void OnStartup()
    {
      _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
      Executor = action =>
                 {
                   if (_sc.IsNull())
                   {
                     action();
                     return;
                   }

                   _sc.Send(state =>
                           {
                             action();
                             _sc.OperationCompleted();
                           }, null);
                 };
    }

    public static void OnMainThread(Action action)
    {
      Executor.Invoke(action);
    }
  }
}