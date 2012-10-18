using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using Contrive.Common.Extensions;

namespace Contrive.Common
{
  public class Cryptographer : ICryptographer
  {
    public Cryptographer(byte[] decryptionKey, Type algorithmType)
    {
      Verify.NotEmpty(decryptionKey, "decryptionKey");

      _key = decryptionKey;
      _algorithmType = algorithmType;
    }

    const int TOKEN_SIZE = 16;
    const int SALT_SIZE = 64;
    const int VALIDATION_KEY_SIZE = 64;
    const int DECRYPTION_KEY_SIZE = 32;

    readonly byte[] _key;
    readonly Type _algorithmType;

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
      return "{0}{1}".FormatWith(password, salt).CalculateHash();
    }

    public string Encrypt(string plainText)
    {
      Verify.NotEmpty(plainText, "plainText");

      byte[] outputBuffer;

      using (var algorithm = _algorithmType.Create<SymmetricAlgorithm>())
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

      using (var algorithm = _algorithmType.Create<SymmetricAlgorithm>())
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
  }
}