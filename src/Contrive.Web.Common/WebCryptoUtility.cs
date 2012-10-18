using System.Text;
using System.Web.Security;
using Contrive.Common;

namespace Contrive.Web.Common
{
  public class WebCryptoUtility : ICryptoUtility
  {
    public string Encrypt(string dataToEncrypt)
    {
      return Encode(dataToEncrypt, MachineKeyProtection.Encryption);
    }

    public string Decrypt(string encryptedData)
    {
      return Decode(encryptedData, MachineKeyProtection.Encryption);
    }

    public string Hash(string dataToHash)
    {
      return Encode(dataToHash, MachineKeyProtection.Validation);
    }

    public string ValidateHash(string hashedData)
    {
      return Decode(hashedData, MachineKeyProtection.Validation);
    }

    public string EncryptAndHash(string dataToEncrypt)
    {
      return Encode(dataToEncrypt, MachineKeyProtection.All);
    }

    public string DecryptHashed(string encryptedData)
    {
      return Decode(encryptedData, MachineKeyProtection.All);
    }

    static string Encode(string input, MachineKeyProtection protectionType)
    {
      var bytesToEncode = Encoding.Unicode.GetBytes(input);

      var output = MachineKey.Encode(bytesToEncode, protectionType);

      return output;
    }

    static string Decode(string input, MachineKeyProtection protectionType)
    {
      var decodedBytes = MachineKey.Decode(input, protectionType);

      var output = Encoding.Unicode.GetString(decodedBytes);

      return output;
    }
  }
}