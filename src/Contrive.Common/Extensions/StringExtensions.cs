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
    public static string CalculateSha512Hash(this string value)
    {
      Verify.NotEmpty(value, "value");

      using (var sha512 = SHA512.Create())
      {
        var data = Encoding.UTF8.GetBytes(value);
        var hash = sha512.ComputeHash(data);

        return hash.ToBase64();
      }
    }

    [DebuggerStepThrough]
    public static string CalculateMd5Hash(this string value)
    {
      Verify.NotEmpty(value, "value");

      using (var md5 = MD5.Create())
      {
        var data = Encoding.UTF8.GetBytes(value);
        var hash = md5.ComputeHash(data);

        return hash.ToBase64();
      }
    }

    [DebuggerStepThrough]
    public static string Base64ToHex(this string base64)
    {
      var hashBytes = Convert.FromBase64String(base64);

      return hashBytes.ToHex();
    }

    [DebuggerStepThrough]
    public static string ToHex(this byte[] input)
    {
      var hex = new StringBuilder();

      foreach (var value in input) hex.AppendFormat("{0:X2}", value);

      return hex.ToString();
    }

    [DebuggerStepThrough]
    public static string ToBase64(this byte[] input)
    {
      return Convert.ToBase64String(input);
    }

    [DebuggerStepThrough]
    public static T ToEnum<T>(this string value, T defaultValue) where T : IComparable, IFormattable
    {
      var convertedValue = defaultValue;

      if (value.IsNotBlank())
      {
        try
        {
          convertedValue = (T) Enum.Parse(typeof (T), value.Trim(), true);
        }
        catch (ArgumentException) {}
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