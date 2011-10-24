using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Text.RegularExpressions;
using Contrive.Core.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Core
{
  public class UserService : IUserService
  {
    public UserService(IUserRepository users,
                                 ICryptographer cryptographer,
                                 IConfigurationProvider configurationProvider)
    {
      _users = users;
      _cryptographer = cryptographer;
      _configurationProvider = configurationProvider;
      Initialize(_configurationProvider.UserManagerConfiguration);
    }

    const int MAX_HASHED_PASSWORD_LENGTH = 128;
    const int DEFAULT_NUM_PASSWORD_FAILURES = 0;
    const int ONE_DAY_IN_MINUTES = 24 * 60;
    readonly IConfigurationProvider _configurationProvider;

    readonly ICryptographer _cryptographer;
    readonly IUserRepository _users;
    public string Realm { get; private set; }

    public string ApplicationName { get; set; }

    public bool EnablePasswordRetrieval { get; private set; }

    public bool EnablePasswordReset { get; private set; }

    public bool RequiresQuestionAndAnswer { get; private set; }

    public int MaxInvalidPasswordAttempts { get; private set; }

    public int PasswordAttemptWindow { get; private set; }

    public bool RequiresUniqueEmail { get; private set; }

    public UserPasswordFormat PasswordFormat { get; private set; }

    public int MinRequiredPasswordLength { get; private set; }

    public int MinRequiredNonAlphanumericCharacters { get; private set; }

    public string PasswordStrengthRegularExpression { get; private set; }

    public int MinPasswordLength
    {
      get { return MinRequiredPasswordLength; }
    }

    public bool ValidateUser(string userName, string password)
    {
      Verify.NotEmpty(userName, "userName");
      Verify.NotEmpty(password, "password");

      var user = GetUser(userName);

      return VerifyUser(user, password);
    }

    public UserCreateStatus CreateUser(string userName, string password, string email)
    {
      Verify.NotEmpty(userName, "userName");
      Verify.NotEmpty(password, "password");
      Verify.NotEmpty(email, "email");

      return CreateUser(userName, password, email, null, null, true, null);
    }

    public UserCreateStatus CreateUser(string userName,
                                       string password,
                                       string email,
                                       string passwordQuestion,
                                       string passwordAnswer,
                                       bool isApproved,
                                       object providerUserKey)
    {
      Verify.NotEmpty(userName, "userName");
      Verify.NotEmpty(password, "password");
      Verify.NotEmpty(email, "email");

      if (RequiresQuestionAndAnswer)
      {
        Verify.NotEmpty(passwordQuestion, "passwordQuestion");
        Verify.NotEmpty(passwordAnswer, "passwordAnswer");
      }

      if (!IsValidPassword(password))
        return UserCreateStatus.InvalidPassword;

      if (RequiresUniqueEmail)
      {
        var emailUser = GetUserByEmail(email);
        if (emailUser != null)
          return UserCreateStatus.DuplicateEmail;
      }

      var existingUser = GetUser(userName);

      if (existingUser != null)
        return UserCreateStatus.DuplicateUserName;

      var passwordSalt = _cryptographer.GenerateSalt();

      var newUser = ServiceLocator.Current.GetInstance<IUser>();

      newUser.Id = Guid.NewGuid();
      newUser.Username = userName;
      newUser.PasswordSalt = passwordSalt;
      newUser.Email = email;
      newUser.IsApproved = isApproved;
      // TODO: HAS 10/23/2011 Read setting for require confirmation.
      newUser.IsConfirmed = true;
      newUser.DateCreated = DateTime.UtcNow;
      newUser.IsLockedOut = false;
      newUser.LastLockedOutDate = DateTime.MinValue;

      SetPasswordFor(newUser, password);

      _users.Insert(newUser);
      _users.SaveChanges();

      return UserCreateStatus.Success;
    }

    public string CreateAccount(string userName, string password, string email, bool requireConfirmationToken = false)
    {
      string token = null;

      if (requireConfirmationToken)
        token = _cryptographer.GenerateToken();

      UserCreateStatus status = CreateUser(userName, password, email);

      if (status != UserCreateStatus.Success) throw new CreateUserException(status);

      var newUser = GetUser(userName);

      newUser.IsConfirmed = !requireConfirmationToken;
      newUser.ConfirmationToken = token;

      _users.SaveChanges();

      return token;
    }

    public bool ChangePassword(string userName, string oldPassword, string newPassword)
    {
      Verify.NotEmpty(userName, "userName");
      Verify.NotEmpty(oldPassword, "oldPassword");
      Verify.NotEmpty(newPassword, "newPassword");

      var user = GetUser(userName);

      if (user == null)
        return false;

      bool verificationSucceeded = VerifyPassword(user, oldPassword);

      if (verificationSucceeded)
        SetPasswordFor(user, newPassword);

      _users.SaveChanges();

      return verificationSucceeded;
    }

    public IUser GetUser(string userName)
    {
      Verify.NotEmpty(userName, "userName");

      return _users.FirstOrDefault(u => u.Username == userName);
    }

    public bool IsValidPassword(string password)
    {
      if (password.Length < MinRequiredPasswordLength)
        return false;

      if (MinRequiredNonAlphanumericCharacters > 0)
      {
        int nonAlpahNumericCharsCount = Regex.Matches(password, "[^a-zA-Z0-9]").Count;
        if (nonAlpahNumericCharsCount < MinRequiredNonAlphanumericCharacters)
          return false;
      }

      if (!PasswordStrengthRegularExpression.IsEmpty())
      {
        if (!Regex.IsMatch(password, PasswordStrengthRegularExpression))
          return false;
      }

      return true;
    }

    public bool VerifyUser(IUser user, string password)
    {
      if (user == null)
        return false;

      if (!user.IsConfirmed)
        return false;

      var verified = VerifyPassword(user, password);

      _users.Update(user);
      _users.SaveChanges();

      return verified;
    }

    public string GeneratePasswordResetToken(string userName)
    {
      return GeneratePasswordResetToken(userName, ONE_DAY_IN_MINUTES);
    }

    public bool ConfirmAccount(string accountConfirmationToken)
    {
      Verify.NotEmpty(accountConfirmationToken, "accountConfirmationToken");

      var user = _users.FirstOrDefault(u => u.ConfirmationToken == accountConfirmationToken);

      if (user != null)
      {
        user.IsConfirmed = true;
        _users.SaveChanges();
        return true;
      }
      return false;
    }

    public bool DeleteAccount(string userName)
    {
      Verify.NotEmpty(userName, "userName");

      var user = GetUser(userName);

      if (user == null)
        return false;

      _users.Delete(user);

      _users.SaveChanges();
      return true;
    }

    public bool IsConfirmed(string userName)
    {
      Verify.NotEmpty(userName, "userName");

      var user = GetUser(userName);

      if (user == null)
        return false;

      if (user.IsConfirmed)
        return true;

      return false;
    }

    public string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow)
    {
      Verify.NotEmpty(userName, "userName");

      var user = GetUser(userName);

      if (user == null)
        throw new InvalidOperationException(String.Format("User not found: {0}", userName));

      if (!user.IsConfirmed)
        throw new InvalidOperationException(String.Format("User not found: {0}", userName));

      string token = user.PasswordVerificationTokenExpirationDate > DateTime.UtcNow
                       ? user.PasswordVerificationToken
                       : _cryptographer.GenerateToken();

      user.PasswordVerificationToken = token;
      user.PasswordVerificationTokenExpirationDate = DateTime.UtcNow.AddMinutes(tokenExpirationInMinutesFromNow);

      _users.SaveChanges();

      return token;
    }

    public bool ResetPasswordWithToken(string token, string newPassword)
    {
      Verify.NotEmpty(newPassword, "newPassword");

      if (!IsValidPassword(newPassword))
        throw new CreateUserException(UserCreateStatus.InvalidPassword);

      var user = _users
        .FirstOrDefault(u =>
          u.PasswordVerificationToken == token
          && u.PasswordVerificationTokenExpirationDate > DateTime.UtcNow);

      var validUser = user != null;

      if (validUser)
        SetPasswordFor(user, newPassword);

      _users.SaveChanges();

      return validUser;
    }

    public string ValidateUserExtended(string userNameOrEmail, string password)
    {
      Verify.NotEmpty(userNameOrEmail, "userNameOrEmail");
      Verify.NotEmpty(password, "password");

      IUser user = GetUser(userNameOrEmail) ??
                  GetUserByEmail(userNameOrEmail);

      if (user == null) return "";

      return VerifyUser(user, password) ? user.Username : String.Empty;
    }

    public DateTime GetPasswordChangedDate(string userName)
    {
      Verify.NotEmpty(userName, "userName");

      var user = GetUser(userName);

      if (user == null)
        throw new InvalidOperationException(String.Format("User not found: {0}", userName));

      return user.PasswordChangedDate.GetValueOrDefault();
    }

    public DateTime GetCreateDate(string userName)
    {
      Verify.NotEmpty(userName, "userName");

      var user = GetUser(userName);

      if (user == null)
        throw new InvalidOperationException(String.Format("User not found: {0}", userName));

      return user.DateCreated.GetValueOrDefault();
    }

    public int GetPasswordFailuresSinceLastSuccess(string userName)
    {
      Verify.NotEmpty(userName, "userName");

      var user = GetUser(userName);

      if (user == null)
        throw new InvalidOperationException(String.Format("User not found: {0}", userName));

      return user.FailedPasswordAttemptCount;
    }

    public Guid GetUserIdFromPasswordResetToken(string token)
    {
      Verify.NotEmpty(token, "token");

      var result = _users.FirstOrDefault(user => user.PasswordVerificationToken == token);

      return result != null ? result.Id : Guid.Empty;
    }

    public DateTime GetLastPasswordFailureDate(string userName)
    {
      Verify.NotEmpty(userName, "userName");

      var user = GetUser(userName);

      if (user == null)
        throw new InvalidOperationException(String.Format("User not found: {0}", userName));

      return user.LastPasswordFailureDate.GetValueOrDefault();
    }

    string GetAuthDigest(string userName, string password)
    {
      // a)
      // A1 = unq(username-value) ":" unq(realm-value) ":" passwd
      var a1 = String.Format("{0}:{1}:{2}", userName, Realm, password);

      // b)
      // HA1 = MD5(A1)
      return _cryptographer.ComputeMd5HashAsHex(a1);
    }

    IUser GetUserByEmail(string emailAddress)
    {
      Verify.NotEmpty(emailAddress, "emailAddress");

      return _users.FirstOrDefault(u => u.Email == emailAddress);
    }

    void Initialize(NameValueCollection config)
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

      Realm = GetConfigValue(_configurationProvider.AppSettings["HTTP.Realm"], "Application");
    }

    string GetConfigValue(string configValue, string defaultValue)
    {
      return configValue.IsEmpty() ? defaultValue : configValue;
    }

    string EncodePassword(string password, string passwordSalt)
    {
      string encodedPassword;

      switch (PasswordFormat)
      {
        //case UserPasswordFormat.Clear:
        //  break;
        //case UserPasswordFormat.Encrypted:
        //  var encryptedPassword = EncryptPassword(passwordBytes);
        //  encodedPassword = Convert.ToBase64String(encryptedPassword);
        //  break;
        case UserPasswordFormat.Hashed:
          encodedPassword = _cryptographer.GetPasswordHash(password, passwordSalt);
          break;
        default:
          throw new ProviderException("Unsupported password format.");
      }

      return encodedPassword;
    }

    bool VerifyPassword(IUser user, string password)
    {
      if (user.Password.IsNull()) return false;

      var encodedPassword = EncodePassword(password, user.PasswordSalt);

      var verified = encodedPassword.Equals(user.Password);

      var currentDate = DateTime.UtcNow;
      if (verified)
      {
        user.FailedPasswordAttemptCount = DEFAULT_NUM_PASSWORD_FAILURES;
        user.LastActivityDate = currentDate;
        user.LastPasswordFailureDate = DateTime.MinValue;
      }
      else
      {
        int failures = user.FailedPasswordAttemptCount;

        if (failures != -1)
        {
          user.FailedPasswordAttemptCount += 1;
          user.LastPasswordFailureDate = currentDate;
        }
      }

      return verified;
    }

    void SetPasswordFor(IUser user, string password)
    {
      if (!IsValidPassword(password))
        throw new ArgumentException("Password is invalid");

      var newEncodedPassword = EncodePassword(password, user.PasswordSalt);

      if (newEncodedPassword.Length > MAX_HASHED_PASSWORD_LENGTH)
        throw new ArgumentException("Password too long");

      user.Password = newEncodedPassword;
      user.AuthDigest = GetAuthDigest(user.Username, password);
      user.PasswordVerificationToken = null;
      user.PasswordVerificationTokenExpirationDate = null;

      var currentDate = DateTime.UtcNow;

      user.LastActivityDate = currentDate;
      user.PasswordChangedDate = currentDate;
    }
  }
}