using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Contrive.Common.Extensions
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
      ActionExtensions.Executor = (action, uponCompletion) =>
                                  {
                                    var worker = new BackgroundWorker();
                                    worker.DoWork += (sender, e) => action();
                                    worker.RunWorkerCompleted += (sender, e) => uponCompletion();
                                    worker.WorkerReportsProgress = false;
                                    worker.WorkerSupportsCancellation = false;
                                    worker.RunWorkerAsync();
                                  };
    }
  }
}