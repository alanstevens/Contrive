using System;
using System.Data;
using Contrive.Common.Extensions;

namespace Contrive.Data.Common
{
  public static class DataRowExtensions
  {
    public static T GetValue<T>(this DataRow row, string columnName)
    {
      var result = default(T);
      var value = row[columnName];
      value = value.IsDBNull() ? null : value;
      if (value.IsNotNull()) result = value.As<T>();
      return result;
    }

    public static string GetString(this DataRow row, string columnName)
    {
      return GetValue<string>(row, columnName);
    }

    public static int GetInt(this DataRow row, string columnName)
    {
      return GetValue<int>(row, columnName);
    }

    public static float GetFloat(this DataRow row, string columnName)
    {
      return GetValue<float>(row, columnName);
    }

    public static DateTime GetDateTime(this DataRow row, string columnName)
    {
      return GetValue<DateTime>(row, columnName);
    }

    public static decimal GetDecimal(this DataRow row, string columnName)
    {
      return GetValue<decimal>(row, columnName);
    }

    public static bool GetBoolean(this DataRow row, string columnName)
    {
      return GetValue<bool>(row, columnName);
    }

    public static Guid GetGuid(this DataRow row, string columnName)
    {
      return GetValue<Guid>(row, columnName);
    }
  }
}