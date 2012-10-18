using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Contrive.Common.Extensions;

namespace Contrive.Common
{
  public class Cryptographer : CryptographerBase
  {
    public Cryptographer(byte[] encryptionKey, Type encryptionAlgorithm, byte[] validationKey, Type validationAlgorithm)
    {
      Verify.NotEmpty(encryptionKey, "decryptionKey");
      _encryptionAlgorithm = encryptionAlgorithm;
      _validationKey = validationKey;
      _validationAlgorithm = validationAlgorithm;
      _encryptionKey = encryptionKey;
    }

    readonly Type _encryptionAlgorithm;
    readonly byte[] _encryptionKey;
    readonly Type _validationAlgorithm;
    readonly byte[] _validationKey;
    int _hashSize;

    int HashSize
    {
      get
      {
        if (_hashSize == 0)
        {
          using (var hashAlgorithm = _validationAlgorithm.Create<HashAlgorithm>())
          {
            _hashSize = hashAlgorithm.HashSize;
          }
        }
        return _hashSize;
      }
    }

    protected override string Encode(string input, Protection protectionOption)
    {
      var data = Encoding.Unicode.GetBytes(input);

      if (data == null) throw new ArgumentNullException("data");
      if (protectionOption == Protection.All || protectionOption == Protection.Validation)
      {
        var numArray1 = Hash(data);
        var numArray2 = new byte[numArray1.Length + data.Length];
        Buffer.BlockCopy(data, 0, numArray2, 0, data.Length);
        Buffer.BlockCopy(numArray1, 0, numArray2, data.Length, numArray1.Length);
        data = numArray2;
      }
      if (protectionOption == Protection.All || protectionOption == Protection.Encryption) data = Protect(data);
      return data.ToHex();
    }

    protected override string Decode(string encodedData, Protection protectionOption)
    {
      if (encodedData == null) throw new ArgumentNullException("encodedData");
      if (encodedData.Length%2 != 0) throw new ArgumentException(null, "encodedData");
      byte[] buffer;

      try
      {
        buffer = encodedData.HexToBinary();
      }
      catch
      {
        throw new ArgumentException(null, "encodedData");
      }

      if (buffer == null || buffer.Length < 1) throw new ArgumentException(null, "encodedData");

      if (protectionOption == Protection.All || protectionOption == Protection.Encryption)
      {
        buffer = Unprotect(buffer);
        if (buffer == null) return null;
      }

      if (protectionOption == Protection.All || protectionOption == Protection.Validation)
      {
        if (buffer.Length < HashSize) return null;
        var bufferCopy = buffer;
        buffer = new byte[bufferCopy.Length - HashSize];
        Buffer.BlockCopy(bufferCopy, 0, buffer, 0, buffer.Length);

        var hashValue = Hash(buffer);

        if (hashValue == null || hashValue.Length != HashSize) return null;

        for (var index = 0; index < hashValue.Length; ++index) if (hashValue[index] != bufferCopy[buffer.Length + index]) return null;
      }

      return Encoding.Unicode.GetString(buffer);
    }

    byte[] Protect(byte[] buffer)
    {
      Verify.NotEmpty(buffer, "buffer");

      byte[] outputBuffer;

      using (var algorithm = _encryptionAlgorithm.Create<SymmetricAlgorithm>())
      {
        using (var ms = new MemoryStream())
        {
          algorithm.GenerateIV();
          algorithm.Key = _encryptionKey;

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

      using (var algorithm = _encryptionAlgorithm.Create<SymmetricAlgorithm>())
      {
        try
        {
          var inputVectorBuffer = new byte[algorithm.IV.Length];

          Array.Copy(inputBuffer, inputVectorBuffer, inputVectorBuffer.Length);

          algorithm.IV = inputVectorBuffer;
          algorithm.Key = _encryptionKey;

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

    byte[] Hash(byte[] buffer)
    {
      KeyedHashAlgorithm keyedHashAlgorithm = null;
      HashAlgorithm hashAlgorithm = null;

      keyedHashAlgorithm = _validationAlgorithm.Create<KeyedHashAlgorithm>();

      if (keyedHashAlgorithm.IsNull()) hashAlgorithm = _validationAlgorithm.Create<HashAlgorithm>();

      if (hashAlgorithm.IsNull()) return HashKeyed(keyedHashAlgorithm, buffer, _validationKey);

      return HashNonKeyed(hashAlgorithm, buffer, _validationKey);
    }

    static byte[] HashNonKeyed(HashAlgorithm algorithm, byte[] buffer, byte[] validationKey)
    {
      var length = buffer.Length + validationKey.Length;
      var newBuffer = new byte[length];
      Buffer.BlockCopy(buffer, 0, newBuffer, 0, buffer.Length);
      Buffer.BlockCopy(validationKey, 0, newBuffer, buffer.Length, validationKey.Length);
      return algorithm.ComputeHash(newBuffer);
    }

    static byte[] HashKeyed(KeyedHashAlgorithm algorithm, byte[] buffer, byte[] validationKey)
    {
      algorithm.Key = validationKey;
      return algorithm.ComputeHash(buffer);
    }
  }
}