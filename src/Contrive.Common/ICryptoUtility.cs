namespace Contrive.Common
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
}