namespace Contrive.Common
{
  public interface ICryptographer
  {
    string GenerateSalt();

    string GenerateToken();

    string CalculatePasswordHash(string password, string salt);

    string ComputeSha512HashAsBase64(string valueToHash);

    string ComputeMd5HashAsHex(string valueToHash);

    string Encrypt(string plainText);

    string Decrypt(string encryptedText);
  }
}