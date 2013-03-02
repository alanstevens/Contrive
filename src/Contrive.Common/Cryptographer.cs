using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Contrive.Common.Extensions;

namespace Contrive.Common
{
    public class Cryptographer<ENCRYPTION_ALGORITHM, HASH_ALGORITM> : CryptographerBase
        where ENCRYPTION_ALGORITHM : SymmetricAlgorithm, new()
        where HASH_ALGORITM : HashAlgorithm, new()
    {
        public Cryptographer(byte[] encryptionKey, byte[] hmacKey)
        {
            Verify.NotEmpty(encryptionKey, "encryptionKey");
            Verify.NotEmpty(hmacKey, "hmacKey");
            _hmacKey = hmacKey;
            _encryptionKey = encryptionKey;
        }

        readonly byte[] _encryptionKey;
        readonly byte[] _hmacKey;
        int _hashSize;

        int HashSize
        {
            get
            {
                if (_hashSize == 0)
                {
                    using (var hashAlgorithm = new HASH_ALGORITM())
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

            Verify.NotNull(data, "data");

            if (protectionOption == Protection.All || protectionOption == Protection.Validation)
            {
                var hashData = Hash(data);
                var validationData = new byte[hashData.Length + data.Length];
                Buffer.BlockCopy(data, 0, validationData, 0, data.Length);
                Buffer.BlockCopy(hashData, 0, validationData, data.Length, hashData.Length);
                data = validationData;
            }
            if (protectionOption == Protection.All || protectionOption == Protection.Encryption) data = Protect(data);
            return data.ToHex();
        }

        protected override string Decode(string encodedData, Protection protectionOption)
        {
            Verify.NotNull(encodedData, "encodedData");
            if (encodedData.Length % 2 != 0) throw new ArgumentException(null, "encodedData");
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

                for (var index = 0; index < hashValue.Length; ++index)
                    if (hashValue[index] != bufferCopy[buffer.Length + index])
                        return null;
            }

            return Encoding.UTF8.GetString(buffer);
        }

        byte[] Protect(byte[] buffer)
        {
            Verify.NotEmpty(buffer, "buffer");

            byte[] outputBuffer;

            using (var algorithm = new ENCRYPTION_ALGORITHM())
            {
                using (var ms = new MemoryStream())
                {
                    algorithm.GenerateIV();
                    algorithm.Key = _encryptionKey;

                    ms.Write(algorithm.IV, 0, algorithm.IV.Length);

                    using (var encryptor = algorithm.CreateEncryptor())
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(buffer, 0, buffer.Length);
                            cs.FlushFinalBlock();
                        }
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

            using (var algorithm = new ENCRYPTION_ALGORITHM())
            {
                try
                {
                    var inputVectorBuffer = new byte[algorithm.IV.Length];

                    Array.Copy(inputBuffer, inputVectorBuffer, inputVectorBuffer.Length);

                    Buffer.BlockCopy(inputVectorBuffer, 0, algorithm.IV, 0, inputVectorBuffer.Length);
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
            byte[] hash;

            using (var hashAlgorithm = new HASH_ALGORITM())
            {
                if (hashAlgorithm.Is<KeyedHashAlgorithm>())
                    hash = HashKeyed(hashAlgorithm.As<KeyedHashAlgorithm>(), buffer, _hmacKey);
                else
                    hash = HashNonKeyed(hashAlgorithm, buffer, _hmacKey);
            }

            return hash;
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