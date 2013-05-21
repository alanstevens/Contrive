using System;
using System.Text;
using System.Web.Security;

namespace Contrive.Common.Web
{
    public class WebCryptographer : CryptographerBase
    {
        protected override string Encode(string input, Protection protectionType)
        {
            var bytesToEncode = Encoding.Unicode.GetBytes(input);

#pragma warning disable 612,618
            var output = MachineKey.Encode(bytesToEncode, ConvertProtection(protectionType));
#pragma warning restore 612,618

            return output;
        }

        protected override string Decode(string input, Protection protectionType)
        {
#pragma warning disable 612,618
            var decodedBytes = MachineKey.Decode(input, ConvertProtection(protectionType));
#pragma warning restore 612,618

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