using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Contrive.Common.Extensions;

namespace Contrive.Common.Data
{
    public static class CommandExtensions
    {
        public static IDbCommand Clear(this IDbCommand command)
        {
            command.Parameters.Clear();
            command.CommandText = "";
            command.Transaction = null; // the default
            command.UpdatedRowSource = UpdateRowSource.None; // the default
            command.CommandType = CommandType.Text;
            return command;
        }

        public static IDbCommand SetProcedureName(this IDbCommand command, string procedureName)
        {
            command.CommandType = CommandType.StoredProcedure;

            command.SetCommandText(procedureName);
            return command;
        }

        public static bool IsConnectionClosed(this IDbCommand command)
        {
            return command.Connection.State == ConnectionState.Closed;
        }

        public static IDbCommand SetCommandText(this IDbCommand command, string commandText)
        {
            command.CommandText = commandText.Trim();
            return command;
        }

        public static IDbCommand ExecuteNonQuery(this IDbCommand command, string sql)
        {
            command.SetCommandText(sql).ExecuteNonQuery();
            return command;
        }

        public static T ExecuteScalar<T>(this IDbCommand command)
        {
            return command.ExecuteScalar().DbNullAs<T>();
        }

        public static T ExecuteScalar<T>(this IDbCommand command, string sql)
        {
            return command.ExecuteScalar(sql).DbNullAs<T>();
        }

        public static object ExecuteScalar(this IDbCommand command, string sql)
        {
            return command.SetCommandText(sql).ExecuteScalar();
        }

        public static DataRow ExecuteSingle(this IDbCommand command)
        {
            DataRow returnVal = null;

            var dataTable = ExecuteDataTable(command, CommandBehavior.SingleRow);

            if (dataTable.Rows.Count > 0) returnVal = dataTable.Rows[0];

            return returnVal;
        }

        public static IEnumerable<DataRow> ExecuteEnumerable(this IDbCommand command)
        {
            return command.ExecuteDataTable().AsEnumerable();
        }

        public static IEnumerable<DataRow> ExecuteEnumerable(this IDbCommand command, string sql)
        {
            return command.SetCommandText(sql).ExecuteDataTable().AsEnumerable();
        }

        public static DataTable ExecuteDataTable(this IDbCommand command)
        {
            return ExecuteDataTable(command, CommandBehavior.Default);
        }

        public static DataTable ExecuteDataTable(this IDbCommand command, CommandBehavior behavior)
        {
            // TODO: HAS 05/18/2013 Get this working with no parameters.
            //LogExecution(command);

            var table = new DataTable();
            try
            {
                using (var reader = command.ExecuteReader(behavior))
                {
                    table.Load(reader);
                }
            }
            catch (Exception ex)
            {
                typeof(CommandExtensions).LogException(ex);
                throw;
            }
            return table;
        }

        public static DataTable ExecuteDataTable(this IDbCommand command, string sql)
        {
            return command.SetCommandText(sql).ExecuteDataTable();
        }

        static void LogExecution(IDbCommand command)
        {
            var parameters =
                command.Parameters.Cast<IDataParameter>().Select(p => "{0}: {1}".FormatWith(p.ParameterName, p.Value.ToString()));
            var parameterText = parameters.Aggregate((a, p) => "{0}\n\t{1}".FormatWith(p, a));
            var message = "Calling: {0}\nWith Parameters:\n\t{1}".FormatWith(command.CommandText, parameterText);
            typeof(CommandExtensions).LogInfo(message);
        }

        public static IDbTransaction BeginTransaction(this IDbCommand command)
        {
            return command.Connection.BeginTransaction();
        }

        public static IDbCommand CommitTransaction(this IDbCommand command, IDbTransaction trans)
        {
            try
            {
                if (trans.IsNull()) throw new InvalidOperationException("No Transaction in process.");

                trans.Commit();
            }
            finally
            {
                if ((trans.IsNotNull())) trans.Dispose();
            }
            return command;
        }

        public static IDbCommand RollbackTransaction(this IDbCommand command, IDbTransaction trans)
        {
            if (trans.IsNull()) throw new InvalidOperationException("No Transaction in process.");

            try
            {
                trans.Rollback();
            }
            finally
            {
                if (trans.IsNotNull()) trans.Dispose();
            }
            return command;
        }
    }

    internal static class DbNullExtensions
    {
        internal static T DbNullAs<T>(this object value)
        {
            return value.IsDBNull()
                       ? default(T)
                       : value.As<T>();
        }
    }
}