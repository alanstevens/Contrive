using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using Contrive.Common.Extensions;

namespace Contrive.Common
{
  public class Cryptographer : ICryptographer
  {
    public Cryptographer(string decryptionKey, string algorithmName)
    {
      Verify.NotEmpty(decryptionKey, "decryptionKey");
      Verify.NotEmpty(algorithmName, "algorithmName");
      Verify.AreNotEqual("Auto", algorithmName, "algorithmName");

      _key = HexStringToByteArray(decryptionKey);

      _algorithmName = algorithmName;
    }

    const int TOKEN_SIZE = 16;
    const int SALT_SIZE = 64;
    const int VALIDATION_KEY_SIZE = 64;
    const int DECRYPTION_KEY_SIZE = 32;

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

    public string GenerateSalt()
    {
      return GetRandomBuffer(SALT_SIZE).ToBase64();
    }

    public string GenerateToken()
    {
      return GetRandomBuffer(TOKEN_SIZE).ToBase64();
    }

    public string GenerateValidationKey()
    {
      return GetRandomBuffer(VALIDATION_KEY_SIZE).ToHex();
    }

    public string GenerateDecryptionKey()
    {
      return GetRandomBuffer(DECRYPTION_KEY_SIZE).ToHex();
    }

    public string CalculatePasswordHash(string password, string salt)
    {
      return "{0}{1}".FormatWith(password, salt).CalculateSha512Hash();
    }

    public string Encrypt(string plainText)
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

    public string Decrypt(string encryptedText)
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

    static byte[] GetRandomBuffer(int bufferSize)
    {
      var buffer = new byte[bufferSize];

      using (var rng = new RNGCryptoServiceProvider())
      {
        rng.GetBytes(buffer);
      }
      return buffer;
    }

    static byte[] HexStringToByteArray(String hex)
    {
      Verify.NotEmpty(hex, "hex");
      var numberChars = hex.Length;
      if (numberChars%2 == 1) hex = '0' + hex;
      var bytes = new byte[numberChars/2];
      for (var i = 0; i < numberChars; i += 2) bytes[i/2] = Convert.ToByte(hex.Substring(i, 2), 16);
      return bytes;
    }
  }
}