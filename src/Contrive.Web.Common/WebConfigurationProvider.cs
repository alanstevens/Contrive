using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Configuration;
using Contrive.Common;

namespace Contrive.Web.Common
{
  public class WebConfigurationProvider : ConfigurationProvider, IWebConfigurationProvider
  {
    public string DefaultRedirect
    {
      [DebuggerStepThrough] get { return GetErrorsSection().DefaultRedirect; }
    }

    public Dictionary<int, string> CustomRedirects
    {
      [DebuggerStepThrough]
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
      [DebuggerStepThrough]
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

    CustomErrorsSection GetErrorsSection()
    {
      return GetSection<CustomErrorsSection>("system.web/customErrors");
    }
  }
}