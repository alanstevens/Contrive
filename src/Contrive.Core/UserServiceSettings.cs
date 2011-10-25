using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using Contrive.Core.Extensions;

namespace Contrive.Core
{
  public class UserServiceSettings : IUserServiceSettings
  {
    public UserServiceSettings(NameValueCollection config)
    {
      Verify.NotNull(config, "config");

      ApplicationName = GetConfigValue(config["applicationName"], "/"); //,HostingEnvironment.ApplicationVirtualPath);

      EnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "false"));

      EnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true`"));

      RequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));

      MaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));

      PasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));

      RequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));

      MinRequiredNonAlphanumericCharacters =
        Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "0"));

      MinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "6"));

      PasswordStrengthRegularExpression = GetConfigValue(config["passwordStrengthRegularExpression"], "");

      string format = config["passwordFormat"] ?? "Hashed";

      switch (format)
      {
        case "Hashed":
          PasswordFormat = UserPasswordFormat.Hashed;
          break;
        //case "Encrypted":
        //  _passwordFormat = UserPasswordFormat.Encrypted;
        //  break;
        //case "Clear":
        //  _passwordFormat = UserPasswordFormat.Clear;
        //  break;
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

    public int MinPasswordLength
    {
      get { return MinRequiredPasswordLength; }
    }

    string GetConfigValue(string configValue, string defaultValue)
    {
      return configValue.IsEmpty() ? defaultValue : configValue;
    }
  }
}