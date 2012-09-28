using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Web.Configuration;
using Contrive.Common;
using Contrive.Common.Extensions;
using Contrive.Common.Web;

namespace Contrive.Web.Common
{
  public class WebConfigurationProvider : ConfigurationProvider, IWebConfigurationProvider
  {
    public NameValueCollection MembershipSettings
    {
      get
      {
        var membershipSection = (MembershipSection) WebConfigurationManager.GetSection("system.web/membership");
        var defaultProvider = membershipSection.DefaultProvider;
        return membershipSection.Providers[defaultProvider].Parameters;
      }
    }

    public NameValueCollection RoleManagerSettings
    {
      get
      {
        var roleManagerSection = (RoleManagerSection) WebConfigurationManager.GetSection("system.web/roleManager");
        var defaultProvider = roleManagerSection.DefaultProvider;
        return roleManagerSection.Providers[defaultProvider].Parameters;
      }
    }

    public string MachineKey
    {
      get
      {
        var decryptionKey = GetMachineKeySection().DecryptionKey;

        if (decryptionKey.Contains("AutoGenerate")) throw new ConfigurationErrorsException("Explicit Algorithm Required");

        return decryptionKey;
      }
    }

    public string DecryptionAlgorithm { get { return GetMachineKeySection().Decryption; } }

    public string DefaultRedirect { get { return GetErrorsSection().DefaultRedirect; } }

    public Dictionary<int, string> CustomRedirects
    {
      get
      {
        var customErrorCollection = GetErrorsSection().Errors;
        var redirects = new Dictionary<int, string>();
        if (customErrorCollection.Count > 0)
        {
          foreach (var key in customErrorCollection.AllKeys)
          {
            var customError = customErrorCollection[key];
            redirects.Add(Convert.ToInt32(key), customError.Redirect);
          }
        }
        return redirects;
      }
    }

    public string CustomErrorsMode
    {
      get
      {
        var customErrorsMode = GetErrorsSection().Mode;

        var modeString = "";

        switch (customErrorsMode)
        {
          case System.Web.Configuration.CustomErrorsMode.Off:
            modeString = "Off";
            break;
          case System.Web.Configuration.CustomErrorsMode.On:
            modeString = "On";
            break;
          case System.Web.Configuration.CustomErrorsMode.RemoteOnly:
            modeString = "RemoteOnly";
            break;
        }
        return modeString;
      }
    }

    static MachineKeySection GetMachineKeySection()
    {
      MachineKeySection machineKeySection;
      try
      {
        machineKeySection = ConfigurationManager.GetSection("system.web/machineKey").As<MachineKeySection>();
      }
      catch (Exception ex)
      {
        throw new ConfigurationErrorsException("Machine Key not found", ex);
      }
      return machineKeySection;
    }

    static CustomErrorsSection GetErrorsSection()
    {
      return ConfigurationManager.GetSection("system.web/customErrors").As<CustomErrorsSection>();
    }
  }
}