namespace Contrive.Common
{
  public interface ICryptographer
  {
    string GenerateSalt();

    string GenerateToken();

    string CalculatePasswordHash(string password, string salt);

    string Encrypt(string dataToEncrypt);

    string Decrypt(string encryptedData);

    string Hash(string dataToHash);

    string ValidateHash(string hashedData);

    string EncryptAndHash(string dataToEncrypt);

    string DecryptHashed(string encryptedData);
  }
}