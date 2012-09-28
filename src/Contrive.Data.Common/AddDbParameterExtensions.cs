using System;
using System.Data;
using System.Data.SqlClient;
using Contrive.Common.Extensions;

namespace Contrive.Data.Common
{
  public static class AddDbParameterExtensions
  {
    const int VAR_CHAR_MAX = 4000;

    public static void AddParameter(this IDbCommand command, string name, DbType type)
    {
      AddParameter(command, name, type, null);
    }

    public static void AddParameter(this IDbCommand command, string name, DbType type, object value)
    {
      AddParameter(command, name, type, value, 0);
    }

    public static void AddParameter(this IDbCommand command, string name, DbType type, object value, int size)
    {
      AddParameter(command, name, type, value, size, ParameterDirection.Input);
    }

    public static void AddOutputParameter(this IDbCommand command, string name, DbType paramType, int size = 0)
    {
      AddParameter(command, name, paramType, null, size, ParameterDirection.Output);
    }

    public static void AddParameter(this IDbCommand command,
                                    string name,
                                    DbType type,
                                    object value,
                                    int size,
                                    ParameterDirection direction)
    {
      var parameter = NewParameter(name, type, direction, value, size);

      command.Parameters.Add(parameter);
    }

    static IDataParameter NewParameter(string name,
                                       DbType type,
                                       ParameterDirection direction,
                                       object value = null,
                                       int size = 0,
                                       byte precision = (byte) 0,
                                       byte scale = (byte) 0)
    {
      if (value.IsNull()) value = DBNull.Value;

      if (type == DbType.AnsiString && size == 0) size = VAR_CHAR_MAX;

      return new SqlParameter
             {
               ParameterName = name,
               DbType = type,
               Direction = direction,
               Value = value,
               Size = size,
               Precision = precision,
               Scale = scale
             };
    }
  }
}