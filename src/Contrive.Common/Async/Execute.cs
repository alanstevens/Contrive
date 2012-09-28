using System;
using System.Threading;
using Contrive.Common.Extensions;

namespace Contrive.Common.Async
{
  public class Execute : IStartupTask
  {
    readonly SynchronizationContext _sc;
    public static Action<Action> Executor = action => action();

    public Execute(SynchronizationContext sc)
    {
      _sc = sc;
    }

    public void OnStartup()
    {
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