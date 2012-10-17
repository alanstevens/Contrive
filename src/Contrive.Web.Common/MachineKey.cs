// Type: System.Web.Security.MachineKey
// Assembly: System.Web, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_32\System.Web\v4.0_4.0.0.0__b03f5f7f11d50a3a\System.Web.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Security.Cryptography;
using System.Web.Util;

namespace System.Web.Security
{
  public static class MachineKey
  {
    [Obsolete("This method is obsolete and is only provided for compatibility with existing code. It is recommended that new code use the Protect and Unprotect methods instead.")]
    public static string Encode(byte[] data, MachineKeyProtection protectionOption)
    {
      if (data == null)
        throw new ArgumentNullException("data");
      if (protectionOption == MachineKeyProtection.All || protectionOption == MachineKeyProtection.Validation)
      {
        byte[] numArray1 = MachineKeySection.HashData(data, (byte[]) null, 0, data.Length);
        byte[] numArray2 = new byte[numArray1.Length + data.Length];
        Buffer.BlockCopy((Array) data, 0, (Array) numArray2, 0, data.Length);
        Buffer.BlockCopy((Array) numArray1, 0, (Array) numArray2, data.Length, numArray1.Length);
        data = numArray2;
      }
      if (protectionOption == MachineKeyProtection.All || protectionOption == MachineKeyProtection.Encryption)
        data = MachineKeySection.EncryptOrDecryptData(true, data, (byte[]) null, 0, data.Length, false, false, IVType.Random, !AppSettings.UseLegacyMachineKeyEncryption);
      return CryptoUtil.BinaryToHex(data);
    }

    [Obsolete("This method is obsolete and is only provided for compatibility with existing code. It is recommended that new code use the Protect and Unprotect methods instead.")]
    public static byte[] Decode(string encodedData, MachineKeyProtection protectionOption)
    {
      if (encodedData == null)
        throw new ArgumentNullException("encodedData");
      if (encodedData.Length % 2 != 0)
        throw new ArgumentException((string) null, "encodedData");
      byte[] buf;
      try
      {
        buf = CryptoUtil.HexToBinary(encodedData);
      }
      catch
      {
        throw new ArgumentException((string) null, "encodedData");
      }
      if (buf == null || buf.Length < 1)
        throw new ArgumentException((string) null, "encodedData");
      if (protectionOption == MachineKeyProtection.All || protectionOption == MachineKeyProtection.Encryption)
      {
        buf = MachineKeySection.EncryptOrDecryptData(false, buf, (byte[]) null, 0, buf.Length, false, false, IVType.Random, !AppSettings.UseLegacyMachineKeyEncryption);
        if (buf == null)
          return (byte[]) null;
      }
      if (protectionOption == MachineKeyProtection.All || protectionOption == MachineKeyProtection.Validation)
      {
        if (buf.Length < MachineKeySection.HashSize)
          return (byte[]) null;
        byte[] numArray1 = buf;
        buf = new byte[numArray1.Length - MachineKeySection.HashSize];
        Buffer.BlockCopy((Array) numArray1, 0, (Array) buf, 0, buf.Length);
        byte[] numArray2 = MachineKeySection.HashData(buf, (byte[]) null, 0, buf.Length);
        if (numArray2 == null || numArray2.Length != MachineKeySection.HashSize)
          return (byte[]) null;
        for (int index = 0; index < numArray2.Length; ++index)
        {
          if ((int) numArray2[index] != (int) numArray1[buf.Length + index])
            return (byte[]) null;
        }
      }
      return buf;
    }

    public static byte[] Protect(byte[] userData, params string[] purposes)
    {
      if (userData == null)
        throw new ArgumentNullException("userData");
      if (purposes != null && Enumerable.Any<string>((IEnumerable<string>) purposes, new Func<string, bool>(string.IsNullOrWhiteSpace)))
        throw new ArgumentException(SR.GetString("MachineKey_InvalidPurpose"), "purposes");
      else
        return MachineKey.Protect((ICryptoServiceProvider) AspNetCryptoServiceProvider.Instance, userData, purposes);
    }

    internal static byte[] Protect(ICryptoServiceProvider cryptoServiceProvider, byte[] userData, string[] purposes)
    {
      Purpose purpose = Purpose.User_MachineKey_Protect.AppendSpecificPurposes((IList<string>) purposes);
      return cryptoServiceProvider.GetCryptoService(purpose, CryptoServiceOptions.None).Protect(userData);
    }

    public static byte[] Unprotect(byte[] protectedData, params string[] purposes)
    {
      if (protectedData == null)
        throw new ArgumentNullException("protectedData");
      if (purposes != null && Enumerable.Any<string>((IEnumerable<string>) purposes, new Func<string, bool>(string.IsNullOrWhiteSpace)))
        throw new ArgumentException(SR.GetString("MachineKey_InvalidPurpose"), "purposes");
      else
        return MachineKey.Unprotect((ICryptoServiceProvider) AspNetCryptoServiceProvider.Instance, protectedData, purposes);
    }

    internal static byte[] Unprotect(ICryptoServiceProvider cryptoServiceProvider, byte[] protectedData, string[] purposes)
    {
      Purpose purpose = Purpose.User_MachineKey_Protect.AppendSpecificPurposes((IList<string>) purposes);
      return cryptoServiceProvider.GetCryptoService(purpose, CryptoServiceOptions.None).Unprotect(protectedData);
    }
  }
}
