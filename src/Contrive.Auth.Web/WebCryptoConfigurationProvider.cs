using System;
using System.Configuration;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Web.Configuration;
using Contrive.Common;
using Contrive.Common.Extensions;

namespace Contrive.Auth.Web
{
  public class WebCryptoConfigurationProvider : ConfigurationProvider, ICryptoConfigurationProvider
  {
    public byte[] EncryptionKey
    {
      [DebuggerStepThrough] get { return GetMachineKeySection().DecryptionKey.HexToBinary(); }
    }

    public Type EncryptionAlgorithm
    {
      [DebuggerStepThrough] get { return GetEncryptionAlgorithmType(GetMachineKeySection().Decryption); }
    }

    public byte[] ValidationKey
    {
      [DebuggerStepThrough] get { return GetMachineKeySection().ValidationKey.HexToBinary(); }
    }

    public Type ValidationAlgorithm
    {
      [DebuggerStepThrough] get { return GetValidationAlgorithmType(GetMachineKeySection().Validation); }
    }

    MachineKeySection GetMachineKeySection()
    {
      MachineKeySection machineKeySection;
      try
      {
        machineKeySection = GetSection<MachineKeySection>("system.web/machineKey");
      }
      catch (Exception ex)
      {
        throw new ConfigurationErrorsException("Encryption configuration not found", ex);
      }
      return machineKeySection;
    }

    static Type GetValidationAlgorithmType(MachineKeyValidation enumValue)
    {
      switch (enumValue)
      {
        case MachineKeyValidation.MD5:
          return typeof (MD5CryptoServiceProvider);
        case MachineKeyValidation.SHA1:
          return typeof (SHA1);
        case MachineKeyValidation.TripleDES:
          return typeof (TripleDESCryptoServiceProvider);
        case MachineKeyValidation.AES:
          return typeof (AesCryptoServiceProvider);
        case MachineKeyValidation.HMACSHA256:
          return typeof (HMACSHA256);
        case MachineKeyValidation.HMACSHA384:
          return typeof (HMACSHA384);
        case MachineKeyValidation.HMACSHA512:
          return typeof (HMACSHA512);
        default:
          throw new ArgumentException("Wrong validation enum.");
      }
    }

    static Type GetEncryptionAlgorithmType(string algorithmName)
    {
      switch (algorithmName)
      {
        case "Auto":
        case "AES":
          return typeof (AesCryptoServiceProvider);
        case "3DES":
          return typeof (TripleDESCryptoServiceProvider);
        default:
          var message = "Unrecognized Algorithm Name: {0}".FormatWith(algorithmName);
          throw new ConfigurationErrorsException(message);
      }
    }
  }
}