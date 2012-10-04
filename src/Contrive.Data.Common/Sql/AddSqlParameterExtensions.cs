using System.Data;
using System.Data.SqlClient;

namespace Contrive.Data.Common.Sql
{
  public static class AddSqlParameterExtensions
  {
    static DbType SqlDbType2DbType(SqlDbType type)
    {
      return new SqlParameter { SqlDbType = type }.DbType;
    }

    public static IDbCommand AddParameter(this IDbCommand command, string name, SqlDbType type)
    {
      AddParameter(command, name, type, null, 0);
      return command;
    }

    public static IDbCommand AddParameter(this IDbCommand command, string name, SqlDbType type, object value)
    {
      AddParameter(command, name, type, value, 0);
      return command;
    }

    public static IDbCommand AddParameter(this IDbCommand command, string name, SqlDbType type, object value, int size)
    {
      command.AddParameter(name, SqlDbType2DbType(type), value, size, ParameterDirection.Input);
      return command;
    }

    public static IDbCommand AddOutputParameter(this IDbCommand command, string name, SqlDbType paramType, int size = 0)
    {
      command.AddOutputParameter(name, SqlDbType2DbType(paramType), size);
      return command;
    }
  }
}