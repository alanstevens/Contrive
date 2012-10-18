using System;
using System.Text;
using System.Web.Security;
using Contrive.Common;

namespace Contrive.Web.Common
{
  public class WebCryptographer : CryptographerBase
  {
    protected override string Encode(string input, Protection protectionType)
    {
      var bytesToEncode = Encoding.Unicode.GetBytes(input);

      var output = MachineKey.Encode(bytesToEncode, ConvertProtection(protectionType));

      return output;
    }

    protected override string Decode(string input, Protection protectionType)
    {
      var decodedBytes = MachineKey.Decode(input, ConvertProtection(protectionType));

      var output = Encoding.Unicode.GetString(decodedBytes);

      return output;
    }

    static MachineKeyProtection ConvertProtection(Protection value)
    {
      switch (value)
      {
        case Protection.Validation:
          return MachineKeyProtection.Validation;
        case Protection.Encryption:
          return MachineKeyProtection.Encryption;
        case Protection.All:
          return MachineKeyProtection.All;
        default:
          throw new Exception("Wrong protection enum.");
      }
    }
  }
}