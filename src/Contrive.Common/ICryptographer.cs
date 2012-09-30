namespace Contrive.Common
{
  public interface ICryptographer
  {
    string GenerateSalt();

    string GenerateToken();

    string CalculatePasswordHash(string password, string salt);

    string Encrypt(string plainText);

    string Decrypt(string encryptedText);
  }
}