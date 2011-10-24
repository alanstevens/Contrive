namespace Contrive.Core
{
  public interface ICryptographer
  {
    string GenerateSalt();

    string GenerateToken();

    string ComputeHash(string valueToHash);

    string GetPasswordHash(string password, string salt);

    string ComputeMd5Hash(string input);

    string Encrypt(string plainText);

    string Decrypt(string encryptedText);
  }
}