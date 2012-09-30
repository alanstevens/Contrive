﻿using System;
using System.Data;
using Contrive.Common;
using Contrive.Common.Extensions;

namespace Contrive.Data.Common
{
  public class DataServiceBase: DisposableBase
  {
    WeakReference _commandReference;

    protected IDbCommand GetCommandFor(string procedureName)
    {
      var command = GetCommand();
      command.SetProcedureName(procedureName);
      return command;
    }

    IDbCommand GetCommand()
    {
      IDbCommand command = null;

      if (_commandReference.IsNotNull() && _commandReference.IsAlive) command = _commandReference.Target.As<IDbCommand>();

      if (command.IsNull() || command.Connection.IsNull() || command.IsConnectionClosed())
      {
        command = UnitOfWork.Current.GetCommand();
        _commandReference = new WeakReference(command);
      }
      else
      {
        command.Clear();
      }
      return command;
    }

    protected override void OnDisposing(bool disposing)
    {
      if (!disposing) return;

      if (!_commandReference.IsAlive) return;

      var command = _commandReference.Target.As<IDbCommand>();
      command.Connection.Dispose();
      command.Dispose();
    }
  }
}