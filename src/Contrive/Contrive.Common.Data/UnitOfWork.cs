using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Contrive.Common.Extensions;

namespace Contrive.Common.Data
{
    public class UnitOfWork : DisposableBase
    {
        UnitOfWork()
        {
            _threadId = Thread.CurrentThread.ManagedThreadId;

            var success = false;

            if (_units.ContainsKey(_threadId)) throw new Exception("Duplicate UnitOfWork instances for the current thread id.");

            for (var count = 0; count < 6; count++)
            {
                success = _units.TryAdd(_threadId, this);
                if (success) break;
                Thread.Sleep(1);
            }

            if (success) return;

            var exception = new Exception("UnitOfWork:  Failed to add thread id {0}.".FormatWith(_threadId));
            this.LogException(exception);
            throw exception;
        }

        static readonly ConcurrentDictionary<int, UnitOfWork> _units = new ConcurrentDictionary<int, UnitOfWork>();

        readonly List<IDbCommand> _commands = new List<IDbCommand>();
        readonly int _threadId;

        int _usages;

        public static UnitOfWork Current
        {
            get
            {
                UnitOfWork unitOfWork = null;
                try
                {
                    var threadId = Thread.CurrentThread.ManagedThreadId;

                    unitOfWork = _units.ContainsKey(threadId)
                                     ? _units[threadId]
                                     : new UnitOfWork();

                    return unitOfWork;
                }
                finally
                {
                    if (unitOfWork.IsNotNull()) unitOfWork._usages++;
                }
            }
        }

        public IDbCommand GetCommand()
        {
            var command = CommandProvider.GetCommand();
            _commands.Add(command);

            return command;
        }

        protected override void OnDisposing(bool disposing)
        {
            try
            {
                if (!disposing) return;

                if (_usages > 1) return;

                foreach (var command in _commands) command.Dispose();

                RemoveCurrent();
            }
            finally
            {
                _usages--;
            }
        }

        void RemoveCurrent()
        {
            if (!_units.ContainsKey(_threadId)) return;

            var success = false;
            var count = 0;

            while (!success)
            {
                count++;
                UnitOfWork current;
                success = _units.TryRemove(_threadId, out current);
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