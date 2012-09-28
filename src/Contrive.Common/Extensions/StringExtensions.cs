using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Contrive.Common.Extensions
{
  public static class StringExtensions
  {
    [DebuggerStepThrough]
    public static string ToProperCase(this string value)
    {
      Verify.NotEmpty(value, "value");
      return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
    }

    [DebuggerStepThrough]
    public static bool Blank(this string value)
    {
      return String.IsNullOrEmpty(value);
    }

    [DebuggerStepThrough]
    public static bool IsNotBlank(this string value)
    {
      return !String.IsNullOrEmpty(value);
    }

    [DebuggerStepThrough]
    public static string FormatWith(this string stringFormat, params object[] args)
    {
      return String.Format(CultureInfo.InvariantCulture, stringFormat, args);
    }

    [DebuggerStepThrough]
    public static string CalculateHash(this string value)
    {
      Verify.NotEmpty(value, "value");

      using(var sha512 = SHA512.Create())
      {
        var data = Encoding.UTF8.GetBytes(value);
        var hash = sha512.ComputeHash(data);

        return Convert.ToBase64String(hash);
      }
    }

    [DebuggerStepThrough]
    public static T ToEnum<T>(this string value, T defaultValue) where T : IComparable, IFormattable
    {
      var convertedValue = defaultValue;

      if (value.IsNotBlank())
      {
        try
        {
          convertedValue = (T)Enum.Parse(typeof(T), value.Trim(), true);
        }
        catch (ArgumentException) { }
      }

      return convertedValue;
    }

    [DebuggerStepThrough]
    public static string Join(this IEnumerable<string> values, string separator = null)
    {
      return Join(values.ToArray(), separator);
    }

    [DebuggerStepThrough]
    public static string Join(this string[] values, string separator = null)
    {
      if (separator.IsNull()) separator = ",";
      return String.Join(separator, values);
    }
  }
}