using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Contrive.Common.Extensions;

namespace Contrive.Common.Async
{
  public static class ActionExtensions
  {
    public static Action<Action, Action> Executor = (action, uponCompletion) =>
                                                    {
                                                      action();
                                                      uponCompletion();
                                                    };

    [DebuggerStepThrough]
    public static void RunAsync(this Action action, Action uponCompletion = null)
    {
      if (uponCompletion.IsNull()) uponCompletion = () => { };
      Executor.Invoke(action, uponCompletion);
    }
  }

  public class ActionExtensionsStartupTask: IStartupTask
  {
    public void OnStartup()
    {
      var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

      ActionExtensions.Executor = (action, uponCompletion) =>
                                  {
                                    var worker = new BackgroundWorker();
                                    worker.DoWork += (s, e) => action();
                                    worker.RunWorkerCompleted += (s, e) => uponCompletion();
                                    worker.WorkerReportsProgress = false;
                                    worker.WorkerSupportsCancellation = false;
                                    worker.RunWorkerAsync();
                                  };
    }
  }
}