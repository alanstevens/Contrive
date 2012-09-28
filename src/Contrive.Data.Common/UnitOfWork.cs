using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Contrive.Common;
using Contrive.Common.Extensions;

namespace Contrive.Data.Common
{
  public class UnitOfWork: DisposableBase, IUnitOfWork, IStartupTask
  {
    public UnitOfWork()
    {
      CurrentThread = Thread.CurrentThread;
      if (_units.ContainsKey(CurrentThread)) throw new Exception("Duplicate UnitOfWork instances for the current thread.");

      var success = false;
      while (!success)
      {
        success = _units.TryAdd(CurrentThread, this);
        if (!success) Thread.Sleep(1);
      }
    }

    Thread CurrentThread { get; set; }

    static Func<IUnitOfWork> _current = () => new UnitOfWork();

    static readonly ConcurrentDictionary<Thread, UnitOfWork> _units = new ConcurrentDictionary<Thread, UnitOfWork>();
    
    readonly List<IDbCommand> _commands = new List<IDbCommand>();

    public static IUnitOfWork Current { get { return _current.Invoke(); } }

    public void OnStartup()
    {
      _current = () =>
                 {
                   var unit = _units[Thread.CurrentThread];

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

      RemoveCurrent(CurrentThread);
    }

    static void RemoveCurrent(Thread currentThread)
    {
      if (!_units.ContainsKey(currentThread)) return;

      var success = false;

      while (!success)
      {
        UnitOfWork current;
        success = _units.TryRemove(currentThread, out current);
        if (!success) Thread.Sleep(1);
      }
    }
  }
}