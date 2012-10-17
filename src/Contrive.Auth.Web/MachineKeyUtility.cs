using System.Text;
using System.Web.Security;

namespace Contrive.Auth.Web
{
  public class MachineKeyUtility
  {
    public string Encrypt(string dataToEncrypt)
    {
      return Encode(dataToEncrypt, MachineKeyProtection.Encryption);
    }

    public string Decrypt(string encryptedData)
    {
      return Decode(encryptedData, MachineKeyProtection.Encryption);
    }

    public string EncryptAndHash(string dataToEncrypt)
    {
      return Encode(dataToEncrypt, MachineKeyProtection.All);
    }

    public string DecryptHashed(string encryptedData)
    {
      return Decode(encryptedData, MachineKeyProtection.All);
    }

    public string Hash(string dataToHash)
    {
      return Encode(dataToHash, MachineKeyProtection.Validation);
    }

    public string ValidateHash(string hashedData)
    {
      return Decode(hashedData, MachineKeyProtection.Validation);
    }

    static string Encode(string input, MachineKeyProtection protectionType)
    {
      var byteArray =
        Encoding.Unicode.GetBytes
          (input);

      var output =
        MachineKey.Encode
          (byteArray, protectionType);

      return output;
    }

    static string Decode(string input, MachineKeyProtection protectionType)
    {
      var output =
        Encoding.Unicode.GetString
          (MachineKey.Decode
             (input, protectionType));

      return output;
    }
  }
}