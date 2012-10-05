using System;
using System.Diagnostics;
using System.Linq;

namespace Contrive.Common.Extensions
{
  public static class TypeExtensions
  {
    [DebuggerStepThrough]
    public static bool TryGetInterface<T>(this Type type, out Type interfaceType)
    {
      interfaceType = type.GetInterfaces().FirstOrDefault(t => t == typeof (T));

      return interfaceType != null;
    }

    public static T Create<T>(this Type type)
    {
      return (T) type.Create();
    }

    public static object Create(this Type type)
    {
      return Activator.CreateInstance(type);
    }
  }
}