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
      [DebuggerStepThrough] get { return GetMachineKeySection().DecryptionKey; }
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
  }
}