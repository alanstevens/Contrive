using System.Security.Cryptography;
using Contrive.Common.Extensions;

namespace Contrive.Common
{
  public abstract class CryptographerBase: ICryptographer
  {
    const int TOKEN_SIZE = 16;
    const int SALT_SIZE = 64;
    const int VALIDATION_KEY_SIZE = 64;
    const int DECRYPTION_KEY_SIZE = 32;

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
      return Hash("{0}{1}".FormatWith(password, salt)).HexToBase64();
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

    protected abstract string Encode(string input, Protection protectionOption);

    protected abstract string Decode(string encodedData, Protection protectionOption);

    static byte[] GetRandomBuffer(int bufferSize)
    {
      var buffer = new byte[bufferSize];

      using (var rng = new RNGCryptoServiceProvider())
      {
        rng.GetBytes(buffer);
      }
      return buffer;
    }

    protected enum Protection
    {
      All,
      Encryption,
      Validation
    }
  }
}