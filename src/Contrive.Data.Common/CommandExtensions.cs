using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Contrive.Common.Extensions;

namespace Contrive.Data.Common
{
  public static class CommandExtensions
  {
    public static void Clear(this IDbCommand command)
    {
      command.Parameters.Clear();
      command.CommandText = "";
      command.Transaction = null; // the default
      command.UpdatedRowSource = UpdateRowSource.None; // the default
      command.CommandType = CommandType.Text;
    }

    public static void SetProcedureName(this IDbCommand command, string procedureName)
    {
      command.CommandType = CommandType.StoredProcedure;

      SetCommandText(command, procedureName);
    }

    public static bool IsConnectionClosed(this IDbCommand command)
    {
      return command.Connection.State == ConnectionState.Closed;
    }

    public static void SetCommandText(this IDbCommand command, string commandText)
    {
      command.CommandText = commandText.Trim();
    }

    public static IEnumerable<DataRow> ExecuteEnumerable(this IDbCommand command)
    {
      var table = ExecuteDataTable(command);
      return table.AsEnumerable();
    }

    public static DataTable ExecuteDataTable(this IDbCommand command)
    {
      LogExecution(command);

      var table = new DataTable();
      try
      {
        using (var reader = command.ExecuteReader(CommandBehavior.Default))
        {
          table.Load(reader);
        }
      }
      catch (Exception ex)
      {
        typeof (CommandExtensions).LogException(ex);
        throw;
      }
      return table;
    }

    static void LogExecution(IDbCommand command)
    {
      var parameters =
        command.Parameters.Cast<IDataParameter>().Select(p => "{0}: {1}".FormatWith(p.ParameterName, p.Value.ToString()));
      var parameterText = parameters.Aggregate((a, p) => "{0}\n\t{1}".FormatWith(p, a));
      var message = "Calling: {0}\nWith Parameters:\n\t{1}".FormatWith(command.CommandText, parameterText);
      typeof(CommandExtensions).LogInfo(message);
    }
  }
}