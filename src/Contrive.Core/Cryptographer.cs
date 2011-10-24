using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Contrive.Core
{
  public class Cryptographer : ICryptographer
  {
    public Cryptographer(IConfigurationProvider configurationProvider)
    {
      _key = Convert.FromBase64String(configurationProvider.GetMachineKey());

      _algorithmName = configurationProvider.GetDecryptionAlgorithm();

      if (_algorithmName == "Auto")
        throw new ConfigurationErrorsException("Explicit Algorithm Required");
    }

    const int TOKEN_SIZE = 16;
    const int SALT_SIZE = 64;

    readonly string _algorithmName;
    readonly byte[] _key;

    public string GenerateSalt()
    {
      return GetBuffer(SALT_SIZE);
    }

    public string GenerateToken()
    {
      return GetBuffer(TOKEN_SIZE);
    }

    public string GetPasswordHash(string password, string salt)
    {
      return ComputeHash(password + salt);
    }

    public string ComputeHash(string valueToHash)
    {
      HashAlgorithm algorithm = SHA512.Create();
      byte[] hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(valueToHash));

      return Convert.ToBase64String(hash);
    }

    public string ComputeMd5Hash(string input)
    {
      Encoding enc = new ASCIIEncoding();
      MD5 md5 = new MD5CryptoServiceProvider();
      byte[] bToConvert = md5.ComputeHash(enc.GetBytes(input));
      string md5Hash = "";

      for (int i = 0; i < 16; i++)
        md5Hash += String.Format("{0:x02}", bToConvert[i]);

      return md5Hash;
    }

    static string GetBuffer(int bufferSize)
    {
      byte[] buffer = new byte[bufferSize];

      //using (var rng = new RNGCryptoServiceProvider())
      //{
      //  rng.GetBytes(buffer);
      //  return Convert.ToBase64String(buffer);
      //}
      var rng = new RNGCryptoServiceProvider();
      rng.GetBytes(buffer);
      return Convert.ToBase64String(buffer);
    }

    public string Encrypt(string plainText)
    {
      Verify.NotEmpty(plainText, "plainText");

      byte[] outputBuffer;

      SymmetricAlgorithm algorithm = GetCryptAlgorithm();

      using (var ms = new MemoryStream())
      {
        algorithm.GenerateIV();
        algorithm.Key = _key;

        ms.Write(algorithm.IV, 0, algorithm.IV.Length);

        var encryptor = algorithm.CreateEncryptor();

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        {
          byte[] buffer = Convert.FromBase64String(plainText);
          cs.Write(buffer, 0, buffer.Length);
          cs.FlushFinalBlock();
        }

        outputBuffer = ms.ToArray();
      }

      return Convert.ToBase64String(outputBuffer);
    }

    public string Decrypt(string encryptedText)
    {
      Verify.NotEmpty(encryptedText, "encryptedText");

      byte[] outputBuffer;

      SymmetricAlgorithm algorithm = GetCryptAlgorithm();

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
      catch (FormatException e)
      {
        throw new CryptographicException("The string could not be decoded.", e);
      }

      return Convert.ToBase64String(outputBuffer);
    }

    SymmetricAlgorithm GetCryptAlgorithm()
    {
      SymmetricAlgorithm algorithm;

      switch (_algorithmName)
      {
        case "AES":
          algorithm = new AesCryptoServiceProvider();
          break;
        case "3DES":
          algorithm = new TripleDESCryptoServiceProvider();
          break;
        case "DES":
          algorithm = new DESCryptoServiceProvider();
          break;
        default:
          string message = string.Format(CultureInfo.InvariantCulture, "Unrecognized Algorithm Name: {0}",
                                         _algorithmName);
          throw new ConfigurationErrorsException(message);
      }

      return algorithm;
    }
  }
}