using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Web.Configuration;
using Contrive.Common;

namespace Contrive.Auth.Web
{
  public class WebAuthConfigurationProvider : ConfigurationProvider, IAuthConfigurationProvider
  {
    public NameValueCollection UserServiceConfiguration
    {
      [DebuggerStepThrough] get { return GetMembershipSettings(); }
    }

    public NameValueCollection RoleServiceConfiguration
    {
      [DebuggerStepThrough]
      get
      {
        var roleManagerSection = GetSection<RoleManagerSection>("system.web/roleManager");
        var defaultProvider = roleManagerSection.DefaultProvider;
        return roleManagerSection.Providers[defaultProvider].Parameters;
      }
    }

    public string DecryptionKey
    {
      [DebuggerStepThrough] get { return GetMachineKey(); }
    }

    public string DecryptionAlgorithm
    {
      [DebuggerStepThrough] get { return GetMachineKeySection().Decryption; }
    }

    NameValueCollection GetMembershipSettings()
    {
      var membershipSection = GetSection<MembershipSection>("system.web/membership");
      var defaultProvider = membershipSection.DefaultProvider;
      var settings = membershipSection.Providers[defaultProvider].Parameters;
      settings.Add("HTTP.Realm", AppSettings["HTTP.Realm"]);
      settings.Add("ContriveEmailFrom", AppSettings["ContriveEmailFrom"]);
      settings.Add("ContriveEmailSubject", AppSettings["ContriveEmailSubject"]);
      settings.Add("ContriveEmailTemplatePath", AppSettings["ContriveEmailTemplatePath"]);
      return settings;
    }

    string GetMachineKey()
    {
      var decryptionKey = GetMachineKeySection().DecryptionKey;

      var bytes = HexStringToByteArray(decryptionKey);

      return Convert.ToBase64String(bytes);
    }

    MachineKeySection GetMachineKeySection()
    {
      MachineKeySection machineKeySection;
      try
      {
        machineKeySection = GetSection<MachineKeySection>("system.web/machineKey");
      }
      catch (Exception ex)
      {
        throw new ConfigurationErrorsException("Encryption configuration not found", ex);
      }
      return machineKeySection;
    }

    //static byte[] HexStringToByteArray(string hexString)
    //{
    //  Verify.NotEmpty(hexString, "hexString");

    //  if (hexString.Length%2 == 1) hexString = '0' + hexString;

    //  var buffer = new byte[hexString.Length/2];

    //  for (var i = 0; i < buffer.Length; ++i) buffer[i] = byte.Parse(hexString.Substring(i*2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

    //  return buffer;
    //}

    static byte[] HexStringToByteArray(String hex)
    {
      Verify.NotEmpty(hex, "hex");
      var numberChars = hex.Length;
      if (numberChars%2 == 1) hex = '0' + hex;
      var bytes = new byte[numberChars/2];
      for (var i = 0; i < numberChars; i += 2) bytes[i/2] = Convert.ToByte(hex.Substring(i, 2), 16);
      return bytes;
    }
  }
}