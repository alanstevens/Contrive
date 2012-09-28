using System;
using System.Diagnostics;

namespace Contrive.Common.Extensions
{
  public static class ObjectExtensions
  {
    [DebuggerStepThrough]
    public static bool IsTypeOf<T>(this object value)
    {
      return value.GetType() == typeof(T);
    }

    [DebuggerStepThrough]
    public static bool IsNot<T>(this object value)
    {
      return !(value is T);
    }

    [DebuggerStepThrough]
    public static bool Is<T>(this object value)
    {
      return value is T;
    }

    [DebuggerStepThrough]
    public static bool IsNull(this object value)
    {
      return ReferenceEquals(value, null);
    }

    [DebuggerStepThrough]
    public static bool IsNotNull(this object value)
    {
      return !ReferenceEquals(value, null);
    }

    [DebuggerStepThrough]
    public static T As<T>(this object value)
    {
      var result = default(T);

      // given an empty string, return the default value
      if (value is String && value.ToString().Blank()) return result;

      try
      {
        if (result is ValueType)
        {
          // TODO: HAS 07/10/2011 Complete tests for all default value types.
          if (result is Int32) return (T) (Convert.ToInt32(value) as object);
          if (result is Double) return (T) (Convert.ToDouble(value) as object);
          if (result is Decimal) return (T) (Convert.ToDecimal(value) as object);
          if (result is Boolean) return (T) (Convert.ToBoolean(value) as object);
          if (result is Guid) return (T) (new Guid(value.ToString()) as object);
          if (typeof(T) == typeof(DateTime)) return (T) (DateTime.Parse(value.ToString()) as object);
        }
        result = (T) value;
      }
      catch (Exception ex)
      {
        typeof(ObjectExtensions).LogException(ex);
      }
      return result;
    }
  }
}