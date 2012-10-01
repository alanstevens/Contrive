using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Contrive.Common;
using Contrive.Common.Extensions;

namespace Contrive.Data.Common
{
  public class UnitOfWork : DisposableBase, IUnitOfWork, IStartupTask
  {
    public UnitOfWork()
    {
      CurrentThreadId = Thread.CurrentThread.ManagedThreadId;
      if (_units.ContainsKey(CurrentThreadId)) throw new Exception("Duplicate UnitOfWork instances for the current thread.");

      var success = false;
      while (!success)
      {
        success = _units.TryAdd(CurrentThreadId, this);
        if (!success) Thread.Sleep(1);
      }
    }

    static Func<IUnitOfWork> _current = () => new UnitOfWork();

    static readonly ConcurrentDictionary<int, UnitOfWork> _units = new ConcurrentDictionary<int, UnitOfWork>();

    readonly List<IDbCommand> _commands = new List<IDbCommand>();

    int CurrentThreadId { get; set; }

    public static IUnitOfWork Current { get { return _current.Invoke(); } }

    public void OnStartup()
    {
      _current = () =>
                 {
                   var threadId = Thread.CurrentThread.ManagedThreadId;
                   var unit = _units[threadId];

                   if (unit.IsNull()) throw new Exception("No UnitOfWork found for the current thread.");

                   return unit;
                 };
    }

    public object GetCommandObject()
    {
      var command = CommandProvider.GetCommand();
      _commands.Add(command);

      return command;
    }

    protected override void OnDisposing(bool disposing)
    {
      if (!disposing) return;

      foreach (var command in _commands) command.Dispose();

      RemoveCurrent(CurrentThreadId);
    }

    static void RemoveCurrent(int currentThreadId)
    {
      if (!_units.ContainsKey(currentThreadId)) return;

      var success = false;

      while (!success)
      {
        UnitOfWork current;
        success = _units.TryRemove(currentThreadId, out current);
        if (!success) Thread.Sleep(1);
      }
    }
  }
}