using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Web.Security;
using Contrive.Core;
using Contrive.Core.Extensions;
using Microsoft.Practices.ServiceLocation;

namespace Contrive.Web.Membership
{
  public class ContriveMembershipProvider : MembershipProvider
  {
    protected const int MAX_HASHED_PASSWORD_LENGTH = 128;
    protected const int DEFAULT_NUM_PASSWORD_FAILURES = 0;

    string _applicationName;
    bool _enablePasswordReset;
    bool _enablePasswordRetrieval;
    int _maxInvalidPasswordAttempts;
    int _minRequiredNonAlphanumericCharacters;
    int _minRequiredPasswordLength;
    int _passwordAttemptWindow;
    MembershipPasswordFormat _passwordFormat;
    string _passwordStrengthRegularExpression;
    bool _requiresQuestionAndAnswer;
    bool _requiresUniqueEmail;

    public override string ApplicationName { get { return _applicationName; } set { _applicationName = value; } }

    public override bool EnablePasswordRetrieval { get { return _enablePasswordReset; } }

    public override bool EnablePasswordReset { get { return _enablePasswordRetrieval; } }

    public override bool RequiresQuestionAndAnswer { get { return _requiresQuestionAndAnswer; } }

    public override int MaxInvalidPasswordAttempts { get { return _maxInvalidPasswordAttempts; } }

    public override int PasswordAttemptWindow { get { return _passwordAttemptWindow; } }

    public override bool RequiresUniqueEmail { get { return _requiresUniqueEmail; } }

    public override MembershipPasswordFormat PasswordFormat { get { return _passwordFormat; } }

    public override int MinRequiredPasswordLength { get { return _minRequiredPasswordLength; } }

    public override int MinRequiredNonAlphanumericCharacters { get { return _minRequiredNonAlphanumericCharacters; } }

    public override string PasswordStrengthRegularExpression { get { return _passwordStrengthRegularExpression; } }

    IUserService GetUserService()
    {
      return ServiceLocator.Current.GetInstance<IUserService>();
    }

    public override void Initialize(string name, NameValueCollection config)
    {
      Verify.NotNull(config, "config");

      if (name.IsEmpty()) name = "ContriveMembershipProvider";

      if (config["description"].IsEmpty())
      {
        config.Remove("description");
        config.Add("description", "Contrive Membership Provider");
      }

      // Initialize the abstract base class.
      base.Initialize(name, config);

      var settings = GetUserService().Settings;

      _applicationName = settings.ApplicationName;
      _enablePasswordReset = settings.EnablePasswordReset;
      _enablePasswordRetrieval = settings.EnablePasswordRetrieval;
      _requiresQuestionAndAnswer = settings.RequiresQuestionAndAnswer;
      _maxInvalidPasswordAttempts = settings.MaxInvalidPasswordAttempts;
      _passwordAttemptWindow = settings.PasswordAttemptWindow;
      _requiresUniqueEmail = settings.RequiresUniqueEmail;
      _minRequiredNonAlphanumericCharacters = settings.MinRequiredNonAlphanumericCharacters;
      _minRequiredPasswordLength = settings.MinRequiredPasswordLength;
      _passwordStrengthRegularExpression = settings.PasswordStrengthRegularExpression;
      _passwordFormat = (MembershipPasswordFormat) settings.PasswordFormat;
    }

    public override bool ChangePassword(string userName, string oldPassword, string newPassword)
    {
      var success = false;

      ThrowProviderExceptionOnError(() => success = GetUserService().ChangePassword(userName, oldPassword, newPassword));

      return success;
    }

    public override MembershipUser GetUser(string userName, bool userIsOnline)
    {
      var user = GetUserService().GetUserByUserName(userName);

      if (user == null) return null;

      return new MembershipUser(System.Web.Security.Membership.Provider.Name, userName, user.Id, user.Email, null, null, true, false, user.DateCreated.GetValueOrDefault(), DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
    }

    public override bool ValidateUser(string userName, string password)
    {
      return GetUserService().ValidateUser(userName, password);
    }

    public override MembershipUser CreateUser(string userName, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
    {
      status = (MembershipCreateStatus) GetUserService().CreateUser(userName, password, email, isApproved);

      return status != MembershipCreateStatus.Success ? null : GetUser(userName, false);
    }

    public override bool DeleteUser(string userName, bool deleteAllRelatedData)
    {
      var success = false;

      ThrowProviderExceptionOnError(() => success = GetUserService().DeleteAccount(userName));

      if (deleteAllRelatedData)
      {
        // TODO: HAS 10/25/2011 Delete profile data.
      }
      return success;
    }

    public override string GetUserNameByEmail(string emailAddress)
    {
      return GetUserService().GetUserByEmail(emailAddress).UserName;
    }

    void ThrowProviderExceptionOnError(Action test)
    {
      try
      {
        test();
      }
      catch (InvalidOperationException e)
      {
        throw new ProviderException(e.Message, e);
      }
    }

    public override bool ChangePasswordQuestionAndAnswer(string userName, string password, string newPasswordQuestion, string newPasswordAnswer)
    {
      throw new NotSupportedException();
    }

    public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      throw new NotSupportedException();
    }

    public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      throw new NotSupportedException();
    }

    public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
    {
      throw new NotSupportedException();
    }

    public override int GetNumberOfUsersOnline()
    {
      throw new NotSupportedException();
    }

    public override string GetPassword(string userName, string answer)
    {
      throw new NotSupportedException();
    }

    public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
    {
      throw new NotSupportedException();
    }

    public override string ResetPassword(string userName, string answer)
    {
      throw new NotSupportedException();
    }

    public override bool UnlockUser(string userName)
    {
      throw new NotSupportedException();
    }

    public override void UpdateUser(MembershipUser user)
    {
      throw new NotSupportedException();
    }
  }
}