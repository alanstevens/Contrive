using System;
using System.Data;
using Contrive.Common.Extensions;

namespace Contrive.Data.Common
{
  public static class RetrieveParameterValueExtensions
  {
    public static int GetInt(this IDbCommand command, string name)
    {
      return GetValue<int>(name, command);
    }

    public static bool GetBool(this IDbCommand command, string name)
    {
      return GetValue<bool>(name, command);
    }

    public static string GetString(this IDbCommand command, string name)
    {
      return GetValue<string>(name, command);
    }

    public static Guid GetGuid(this IDbCommand command, string name)
    {
      return GetValue<Guid>(name, command);
    }

    public static char GetChar(this IDbCommand command, string name)
    {
      return GetValue<char>(name, command);
    }

    public static DateTime GetDate(this IDbCommand command, string name)
    {
      return GetValue<DateTime>(name, command);
    }

    public static decimal GetDecimal(this IDbCommand command, string name)
    {
      return GetValue<Decimal>(name, command);
    }

    public static T GetValue<T>(this IDbCommand command, string name)
    {
      return GetValue<T>(name, command);
    }

    static T GetValue<T>(string name, IDbCommand command)
    {
      return GetValue<T>(command.GetParameter(name));
    }

    static T GetValue<T>(IDataParameter parameter)
    {
      var value = parameter.Value;

      return value.IsNotDBNull() ? value.As<T>() : default(T);
    }

    public static IDbDataParameter GetParameter(this IDbCommand command, string name)
    {
      return command.Parameters[name].As<IDbDataParameter>();
    }
  }
}