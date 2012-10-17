using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Configuration;
using System.Web.Security;
using Contrive.Common;
using Contrive.Common.Extensions;

namespace Contrive.Web.Common
{
  public interface ICryptoUtility
  {
    string Encrypt(string dataToEncrypt);

    string Decrypt(string encryptedData);

    string Hash(string dataToHash);

    string ValidateHash(string hashedData);

    string EncryptAndHash(string dataToEncrypt);

    string DecryptHashed(string encryptedData);
  }

  internal class CryptoUtility : ICryptoUtility
  {
    public CryptoUtility(string decryptionKey, string algorithmName)
    {
      Verify.NotEmpty(decryptionKey, "decryptionKey");
      Verify.NotEmpty(algorithmName, "algorithmName");
      Verify.AreNotEqual("Auto", algorithmName, "algorithmName");

      _key = decryptionKey.HexToBinary();

      _algorithmName = algorithmName;
    }

    readonly string _algorithmName;
    readonly byte[] _key;

    Type EncryptionAlgorithmType
    {
      get
      {
        Type algorithm;

        switch (_algorithmName)
        {
          case "AES":
            algorithm = typeof (AesCryptoServiceProvider);
            break;
          case "3DES":
            algorithm = typeof (TripleDESCryptoServiceProvider);
            break;
          case "DES":
            algorithm = typeof (DESCryptoServiceProvider);
            break;
          default:
            var message = "Unrecognized Algorithm Name: {0}".FormatWith(_algorithmName);
            throw new ConfigurationErrorsException(message);
        }

        return algorithm;
      }
    }
    public string Encrypt(string dataToEncrypt)
    {
      return null;
    }

    public string Decrypt(string encryptedData)
    {
      return null;
    }

    public string Hash(string dataToHash)
    {
      return null;
    }

    public string ValidateHash(string hashedData)
    {
      return null;
    }

    public string EncryptAndHash(string dataToEncrypt)
    {
      return null;
    }

    public string DecryptHashed(string encryptedData)
    {
      return null;
    }

    public static string Encode(byte[] data, MachineKeyProtection protectionOption)
    {
      if (data == null) throw new ArgumentNullException("data");
      if (protectionOption == MachineKeyProtection.All || protectionOption == MachineKeyProtection.Validation)
      {
        var numArray1 = data.CalculateHash();
        var numArray2 = new byte[numArray1.Length + data.Length];
        Buffer.BlockCopy(data, 0, numArray2, 0, data.Length);
        Buffer.BlockCopy(numArray1, 0, numArray2, data.Length, numArray1.Length);
        data = numArray2;
      }
      if (protectionOption == MachineKeyProtection.All || protectionOption == MachineKeyProtection.Encryption)
      {
        data = MachineKeySection.EncryptOrDecryptData(true, data, (byte[]) null, 0, data.Length, false, false,
                                                      IVType.Random, !AppSettings.UseLegacyMachineKeyEncryption);
      }
      return data.ToHex();
    }

    public static byte[] Decode(string encodedData, MachineKeyProtection protectionOption)
    {
      if (encodedData == null) throw new ArgumentNullException("encodedData");
      if (encodedData.Length%2 != 0) throw new ArgumentException(null, "encodedData");
      byte[] buf;

      try
      {
        buf = encodedData.HexToBinary();
      }
      catch
      {
        throw new ArgumentException(null, "encodedData");
      }

      if (buf == null || buf.Length < 1) throw new ArgumentException(null, "encodedData");

      if (protectionOption == MachineKeyProtection.All || protectionOption == MachineKeyProtection.Encryption)
      {
        buf = MachineKeySection.EncryptOrDecryptData(false, buf, (byte[]) null, 0, buf.Length, false, false,
                                                     IVType.Random, !AppSettings.UseLegacyMachineKeyEncryption);
        if (buf == null) return null;
      }

      if (protectionOption == MachineKeyProtection.All || protectionOption == MachineKeyProtection.Validation)
      {
        if (buf.Length < MachineKeySection.HashSize) return null;
        var numArray1 = buf;
        buf = new byte[numArray1.Length - MachineKeySection.HashSize];
        Buffer.BlockCopy(numArray1, 0, buf, 0, buf.Length);
        
        var numArray2 = buf.CalculateHash();

        if (numArray2 == null || numArray2.Length != MachineKeySection.HashSize) return null;

        for (var index = 0; index < numArray2.Length; ++index) if (numArray2[index] != numArray1[buf.Length + index]) return null;
      }

      return buf;
    }

    string Encrypt(string plainText)
    {
      Verify.NotEmpty(plainText, "plainText");

      byte[] outputBuffer;

      using (var algorithm = EncryptionAlgorithmType.Create<SymmetricAlgorithm>())
      {
        using (var ms = new MemoryStream())
        {
          algorithm.GenerateIV();
          algorithm.Key = _key;

          ms.Write(algorithm.IV, 0, algorithm.IV.Length);

          var encryptor = algorithm.CreateEncryptor();

          using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
          {
            var buffer = Convert.FromBase64String(plainText);
            cs.Write(buffer, 0, buffer.Length);
            cs.FlushFinalBlock();
          }

          outputBuffer = ms.ToArray();
        }
      }

      return Convert.ToBase64String(outputBuffer);
    }

    string Decrypt(string encryptedText)
    {
      Verify.NotEmpty(encryptedText, "encryptedText");

      byte[] outputBuffer;

      using (var algorithm = EncryptionAlgorithmType.Create<SymmetricAlgorithm>())
      {
        try
        {
          var inputBuffer = Convert.FromBase64String(encryptedText);

          var inputVectorBuffer = new byte[algorithm.IV.Length];

          Array.Copy(inputBuffer, inputVectorBuffer, inputVectorBuffer.Length);

          algorithm.IV = inputVectorBuffer;
          algorithm.Key = _key;

          using (var ms = new MemoryStream())
          {
            using (var cs = new CryptoStream(ms, algorithm.CreateDecryptor(), CryptoStreamMode.Write))
            {
              cs.Write(inputBuffer, inputVectorBuffer.Length, inputBuffer.Length - inputVectorBuffer.Length);
              cs.FlushFinalBlock();
            }

            outputBuffer = ms.ToArray();
          }
        }
        catch (FormatException ex)
        {
          this.LogException(ex);
          throw new CryptographicException("The value could not be decoded.", ex);
        }
      }

      return Convert.ToBase64String(outputBuffer);
  }

  public class WebCryptoUtility : ICryptoUtility
  {
    public string Encrypt(string dataToEncrypt)
    {
      return Encode(dataToEncrypt, MachineKeyProtection.Encryption);
    }

    public string Decrypt(string encryptedData)
    {
      return Decode(encryptedData, MachineKeyProtection.Encryption);
    }

    public string Hash(string dataToHash)
    {
      return Encode(dataToHash, MachineKeyProtection.Validation);
    }

    public string ValidateHash(string hashedData)
    {
      return Decode(hashedData, MachineKeyProtection.Validation);
    }

    public string EncryptAndHash(string dataToEncrypt)
    {
      return Encode(dataToEncrypt, MachineKeyProtection.All);
    }

    public string DecryptHashed(string encryptedData)
    {
      return Decode(encryptedData, MachineKeyProtection.All);
    }

    static string Encode(string input, MachineKeyProtection protectionType)
    {
      var bytesToEncode = Encoding.Unicode.GetBytes(input);

      var output = MachineKey.Encode(bytesToEncode, protectionType);

      return output;
    }

    static string Decode(string input, MachineKeyProtection protectionType)
    {
      var decodedBytes = MachineKey.Decode(input, protectionType);

      var output = Encoding.Unicode.GetString(decodedBytes);

      return output;
    }
  }
}