using System;
using System.Collections.Generic;
using Contrive.Common.Extensions;

namespace Contrive.Common.Async
{
  public class TaskQueue : ITasks, IStartupTask
  {
    public static Action<Action, Action> Executor = (action, uponCompletion) =>
                                                    {
                                                      action();
                                                      uponCompletion();
                                                    };

    readonly List<Action> _tasks = new List<Action>();

    int _currentIndex = -1;

    public void OnStartup()
    {
      Executor = (action, uponCompletion) => action.RunAsync(uponCompletion);
    }

    public ITasks Enqueue(Action task)
    {
      _tasks.Add(task);
      return this;
    }

    public ITasks Run()
    {
      if (_currentIndex == -1) ExecuteNext();
      return this;
    }

    void ExecuteNext()
    {
      lock (_tasks)
      {
        _currentIndex++;
        if (_currentIndex == _tasks.Count)
        {
          _tasks.Clear();
          _currentIndex = -1;
        }
        else
        {
          var nextStep = _tasks[_currentIndex];
          Executor.Invoke(nextStep, ExecuteNext);
        }
      }
    }
  }
}