using System;
using System.Threading;

namespace Contrive.Core.Extensions
{
  public static class StringExtensions
  {
    public static string ToProperCase(this string value)
    {
      return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
    }

    public static bool IsEmpty(this string stringValue)
    {
      return string.IsNullOrEmpty(stringValue);
    }

    public static bool IsNotEmpty(this string stringValue)
    {
      return !string.IsNullOrEmpty(stringValue);
    }

    public static string FormatWith(this string stringFormat, params object[] args)
    {
      return String.Format(stringFormat, args);
    }

    /// <summary>
    /// Returns a DateTime value parsed from the <paramref name="dateTimeValue"/> parameter.
    /// </summary>
    /// <param name="dateTimeValue">A valid, parseable DateTime value</param>
    /// <returns>The parsed DateTime value</returns>
    public static DateTime ToDateTime(this string dateTimeValue)
    {
      return DateTime.Parse(dateTimeValue);
    }
  }
}