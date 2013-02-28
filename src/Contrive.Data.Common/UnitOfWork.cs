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
            _syncContext = SynchronizationContext.Current;

            if (_units.ContainsKey(_syncContext)) throw new Exception("Duplicate UnitOfWork instances for the current SynchronizationContext.");

            var success = false;

            for (var count = 0; count < 6; count++)
            {
                success = _units.TryAdd(_syncContext, this);
                if (success) break;
                Thread.Sleep(1);
            }

            if (!success)
            {
                var exception = new Exception("UnitOfWork:  Failed to add synch context.");
                this.LogException(exception);
                throw exception;
            }
        }

        static Func<IUnitOfWork> _current = () => new UnitOfWork();

        static readonly ConcurrentDictionary<SynchronizationContext, UnitOfWork> _units =
            new ConcurrentDictionary<SynchronizationContext, UnitOfWork>();

        readonly List<IDbCommand> _commands = new List<IDbCommand>();
        readonly SynchronizationContext _syncContext;

        public static IUnitOfWork Current { get { return _current.Invoke(); } }

        public void OnStartup()
        {
            _current = () =>
                       {
                           var unit = _units[SynchronizationContext.Current];

                           if (unit.IsNull()) throw new Exception("No UnitOfWork found for the current SynchronizationContext.");

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

            RemoveCurrent();
        }

        void RemoveCurrent()
        {
            if (!_units.ContainsKey(_syncContext)) return;

            var success = false;
            var count = 0;

            while (!success)
            {
                count++;
                UnitOfWork current;
                success = _units.TryRemove(_syncContext, out current);
                if (!success && count < 5) Thread.Sleep(1);
                else
                {
                    success = true; // Well, bugger!
                    this.LogInfo("Failed to remove synch context.");
                }
            }
        }
    }
}