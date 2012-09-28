using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Contrive.Core.Extensions
{
  public static class GenericEnumerableExtensions
  {
    [DebuggerStepThrough]
    public static IEnumerable<T> Each<T>(this IEnumerable<T> values, Action<T> eachAction)
    {
      foreach (var item in values) eachAction(item);

      return values;
    }

    [DebuggerStepThrough]
    public static IEnumerable Each(this IEnumerable values, Action<object> eachAction)
    {
      foreach (var item in values) eachAction(item);

      return values;
    }
  }
}