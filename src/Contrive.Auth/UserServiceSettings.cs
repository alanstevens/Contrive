using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using Contrive.Common;
using Contrive.Common.Extensions;

namespace Contrive.Auth
{
  public class UserServiceSettings : IUserServiceSettings
  {
    public UserServiceSettings(NameValueCollection settings)
    {
      Verify.NotNull(settings, "settings");

      ApplicationName = GetConfigValue(settings["applicationName"], "/"); //,HostingEnvironment.ApplicationVirtualPath);

      EnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(settings["enablePasswordReset"], "false"));

      EnablePasswordReset = Convert.ToBoolean(GetConfigValue(settings["enablePasswordRetrieval"], "true"));

      RequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(settings["requiresQuestionAndAnswer"], "false"));

      MaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(settings["maxInvalidPasswordAttempts"], "5"));

      PasswordAttemptWindow = Convert.ToInt32(GetConfigValue(settings["passwordAttemptWindow"], "10"));

      RequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(settings["requiresUniqueEmail"], "true"));

      MinRequiredNonAlphanumericCharacters =
        Convert.ToInt32(GetConfigValue(settings["minRequiredNonAlphanumericCharacters"], "0"));

      MinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(settings["minRequiredPasswordLength"], "6"));

      PasswordStrengthRegularExpression = GetConfigValue(settings["passwordStrengthRegularExpression"], "");

      Realm = GetConfigValue(settings["HTTP.Realm"], "Application");

      ContriveEmailFrom = GetConfigValue(settings["Contrive.Auth.EmailFrom"], "Application");

      ContriveEmailSubject = GetConfigValue(settings["Contrive.Auth.EmailSubject"], "Password Reset Request");

      ContriveEmailTemplatePath = GetConfigValue(settings["Contrive.Auth.EmailTemplatePath"],
                                                 "~/Content/Contrive/ResetPassword.html");

      var format = settings["passwordFormat"] ?? "Hashed";

      switch (format)
      {
        case "Hashed":
          PasswordFormat = UserPasswordFormat.Hashed;
          break;
        case "Encrypted":
          PasswordFormat = UserPasswordFormat.Encrypted;
          break;
        case "Clear":
          throw new ArgumentException("Clear text password storage is not allowed.");
        default:
          throw new ProviderException("Password format not supported.");
      }
    }

    public string Realm { get; set; }

    public string ApplicationName { get; set; }

    public bool EnablePasswordRetrieval { get; internal set; }

    public bool EnablePasswordReset { get; internal set; }

    public bool RequiresQuestionAndAnswer { get; internal set; }

    public int MaxInvalidPasswordAttempts { get; internal set; }

    public int PasswordAttemptWindow { get; internal set; }

    public bool RequiresUniqueEmail { get; internal set; }

    public UserPasswordFormat PasswordFormat { get; internal set; }

    public int MinRequiredPasswordLength { get; internal set; }

    public int MinRequiredNonAlphanumericCharacters { get; internal set; }

    public string PasswordStrengthRegularExpression { get; internal set; }

    public string ContriveEmailFrom { get; internal set; }

    public string ContriveEmailSubject { get; internal set; }

    public string ContriveEmailTemplatePath { get; internal set; }

    public int MinPasswordLength { get { return MinRequiredPasswordLength; } }

    static string GetConfigValue(string configValue, string defaultValue)
    {
      return configValue.IsEmpty() ? defaultValue : configValue;
    }
  }
}