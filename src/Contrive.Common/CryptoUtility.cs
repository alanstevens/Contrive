using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Contrive.Common.Extensions;

namespace Contrive.Common
{
  internal class CryptoUtility : ICryptoUtility
  {
    public CryptoUtility(string decryptionKey, Type algorithmType, string validationKey, Type validationType)
    {
      _algorithmType = algorithmType;
      _validationKey = validationKey.HexToBinary();
      _validationType = validationType;
      Verify.NotEmpty(decryptionKey, "decryptionKey");

      _key = decryptionKey.HexToBinary();
    }

    readonly Type _algorithmType;
    readonly byte[] _key;
    readonly byte[] _validationKey;
    readonly Type _validationType;
    int _hashSize;

    int HashSize
    {
      get
      {
        if (_hashSize == 0)
        {
          using (var hashAlgorithm = _validationType.Create<HashAlgorithm>())
          {
            _hashSize = hashAlgorithm.HashSize;
          }
        }
        return _hashSize;
      }
    }

    public string Encrypt(string dataToEncrypt)
    {
      return Encode(dataToEncrypt, Protection.Encryption);
    }

    public string Decrypt(string encryptedData)
    {
      return Decode(encryptedData, Protection.Encryption);
    }

    public string Hash(string dataToHash)
    {
      return Encode(dataToHash, Protection.Validation);
    }

    public string ValidateHash(string hashedData)
    {
      return Decode(hashedData, Protection.Validation);
    }

    public string EncryptAndHash(string dataToEncrypt)
    {
      return Encode(dataToEncrypt, Protection.All);
    }

    public string DecryptHashed(string encryptedData)
    {
      return Decode(encryptedData, Protection.All);
    }

    string Encode(string input, Protection protectionOption)
    {
      var data = Encoding.Unicode.GetBytes(input);

      if (data == null) throw new ArgumentNullException("data");
      if (protectionOption == Protection.All || protectionOption == Protection.Validation)
      {
        var numArray1 = HashData(data);
        var numArray2 = new byte[numArray1.Length + data.Length];
        Buffer.BlockCopy(data, 0, numArray2, 0, data.Length);
        Buffer.BlockCopy(numArray1, 0, numArray2, data.Length, numArray1.Length);
        data = numArray2;
      }
      if (protectionOption == Protection.All || protectionOption == Protection.Encryption) data = Protect(data);
      return data.ToHex();
    }

    string Decode(string encodedData, Protection protectionOption)
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

      if (protectionOption == Protection.All || protectionOption == Protection.Encryption)
      {
        buf = Unprotect(buf);
        if (buf == null) return null;
      }

      if (protectionOption == Protection.All || protectionOption == Protection.Validation)
      {
        if (buf.Length < HashSize) return null;
        var numArray1 = buf;
        buf = new byte[numArray1.Length - HashSize];
        Buffer.BlockCopy(numArray1, 0, buf, 0, buf.Length);

        var numArray2 = HashData(buf);

        if (numArray2 == null || numArray2.Length != HashSize) return null;

        for (var index = 0; index < numArray2.Length; ++index) if (numArray2[index] != numArray1[buf.Length + index]) return null;
      }

      return Encoding.Unicode.GetString(buf);
    }

    byte[] Protect(byte[] buffer)
    {
      Verify.NotEmpty(buffer, "buffer");

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
            cs.Write(buffer, 0, buffer.Length);
            cs.FlushFinalBlock();
          }

          outputBuffer = ms.ToArray();
        }
      }

      return outputBuffer;
    }

    byte[] Unprotect(byte[] inputBuffer)
    {
      Verify.NotEmpty(inputBuffer, "inputBuffer");

      byte[] outputBuffer;

      using (var algorithm = _algorithmType.Create<SymmetricAlgorithm>())
      {
        try
        {
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

      return outputBuffer;
    }

    byte[] HashData(byte[] buf)
    {
      KeyedHashAlgorithm keyedHashAlgorithm = null;
      HashAlgorithm hashAlgorithm = null;

      keyedHashAlgorithm = _validationType.Create<KeyedHashAlgorithm>();

      if (keyedHashAlgorithm.IsNull()) hashAlgorithm = _validationType.Create<HashAlgorithm>();

      if (hashAlgorithm.IsNull()) return HashDataUsingKeyedAlgorithm(keyedHashAlgorithm, buf, _validationKey);
      return HashDataUsingNonKeyedAlgorithm(hashAlgorithm, buf, _validationKey);
    }

    static byte[] HashDataUsingNonKeyedAlgorithm(HashAlgorithm hashAlgo, byte[] buf, byte[] validationKey)
    {
      var num = buf.Length + validationKey.Length;
      var array = new byte[num];
      Buffer.BlockCopy(buf, 0, array, 0, buf.Length);
      Buffer.BlockCopy(validationKey, 0, array, buf.Length, validationKey.Length);
      return hashAlgo.ComputeHash(array);
    }

    static byte[] HashDataUsingKeyedAlgorithm(KeyedHashAlgorithm hashAlgo, byte[] buf, byte[] validationKey)
    {
      var num = buf.Length;
      var array = new byte[num];
      Buffer.BlockCopy(buf, 0, array, 0, buf.Length);
      hashAlgo.Key = validationKey;
      return hashAlgo.ComputeHash(array);
    }

    enum Protection
    {
      All,
      Encryption,
      Validation
    }
  }
}