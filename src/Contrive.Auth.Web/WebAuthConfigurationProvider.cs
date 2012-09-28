using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Web.Configuration;
using Contrive.Common;
using Contrive.Common.Extensions;
using Contrive.Web.Common;

namespace Contrive.Auth.Web
{
  public class WebAuthConfigurationProvider : WebConfigurationProvider, IAuthConfigurationProvider
  {
    NameValueCollection UserServiceConfiguration
    {
      [DebuggerStepThrough]
      get
      {
        var membershipSection = WebConfigurationManager.GetSection("system.web/membership").As<MembershipSection>();
        var defaultProvider = membershipSection.DefaultProvider;
        var settings = membershipSection.Providers[defaultProvider].Parameters;
        settings.Add("HTTP.Realm", AppSettings["HTTP.Realm"]);
        settings.Add("ContriveEmailFrom", AppSettings["ContriveEmailFrom"]);
        settings.Add("ContriveEmailSubject", AppSettings["ContriveEmailSubject"]);
        settings.Add("ContriveEmailTemplatePath", AppSettings["ContriveEmailTemplatePath"]);
        return settings;
      }
    }

    public IUserServiceSettings UserServiceSettings
    {
      [DebuggerStepThrough] get { return new UserServiceSettings(UserServiceConfiguration); }
    }

    public NameValueCollection RoleServiceConfiguration
    {
      [DebuggerStepThrough]
      get
      {
        var roleManagerSection = WebConfigurationManager.GetSection("system.web/roleManager").As<RoleManagerSection>();
        var defaultProvider = roleManagerSection.DefaultProvider;
        return roleManagerSection.Providers[defaultProvider].Parameters;
      }
    }

    [DebuggerStepThrough]
    public string GetMachineKey()
    {
      var decryptionKey = GetMachineKeySection().DecryptionKey;

      if (decryptionKey.Contains("AutoGenerate")) throw new ConfigurationErrorsException("Explicit Algorithm Required");

      var bytes = HexStringToByteArray(decryptionKey);

      return Convert.ToBase64String(bytes);
    }

    [DebuggerStepThrough]
    public string GetDecryptionAlgorithm()
    {
      return GetMachineKeySection().Decryption;
    }

    static MachineKeySection GetMachineKeySection()
    {
      return ConfigurationManager.GetSection("system.web/machineKey").As<MachineKeySection>();
    }

    static byte[] HexStringToByteArray(string hexString)
    {
      Verify.NotEmpty(hexString, "hexString");

      if (hexString.Length%2 == 1) hexString = '0' + hexString;

      var buffer = new byte[hexString.Length/2];

      for (var i = 0; i < buffer.Length; ++i) buffer[i] = byte.Parse(hexString.Substring(i*2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

      return buffer;
    }
  }
}