using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Contrive.Common.Extensions;

namespace Contrive.Common.Async
{
  public static class ActionExtensions
  {
    public static Func<Action, Action, Task> Executor = (action, continueWith) =>
                                                    {
                                                      action();
                                                      continueWith();
                                                      var task = new Task(continueWith);
                                                      task.RunSynchronously();
                                                      return task;
                                                    };

    [DebuggerStepThrough]
    public static Task RunAsync(this Action action, Action continueWith = null)
    {
      if (continueWith.IsNull()) continueWith = () => { };
      return Executor.Invoke(action, continueWith);
    }
  }

  public class ActionExtensionsStartupTask: IStartupTask
  {
    public void OnStartup()
    {
      var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
      ActionExtensions.Executor = (action, continueWith) => Task.Factory.StartNew(action).ContinueWith(t => continueWith, taskScheduler);
    }
  }
}