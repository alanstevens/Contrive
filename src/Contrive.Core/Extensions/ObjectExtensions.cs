using System;

namespace Contrive.Core.Extensions
{
  public static class ObjectExtensions
  {
    public static T As<T>(this object o)
    {
      T value = default(T);

      if (o is String && o.ToString().IsEmpty()) return value;

      try
      {
        if (value is ValueType)
        {
          // TODO: HAS 07/10/2011 Complete tests for all default value types.
          if (value is Int32) return (T)(Convert.ToInt32(o) as object);
          if (value is Double) return (T)(Convert.ToDouble(o) as object);
          if (value is Decimal) return (T)(Convert.ToDecimal(o) as object);
          if (value is Boolean) return (T)(Convert.ToBoolean(o) as object);
          if (value is Guid) return (T)(Guid.Parse(o.ToString()) as object);
        }
        value = (T)o;
      }
      catch (Exception)
      {
      }
      return value;
    }

    public static bool IsNull(this object obj)
    {
      return ReferenceEquals(obj, null);
    }

    public static bool IsNotNull(this object obj)
    {
      return !ReferenceEquals(obj, null);
    }
  }
}