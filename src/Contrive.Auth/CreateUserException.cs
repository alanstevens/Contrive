using System;
using System.Runtime;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Contrive.Auth.Properties;

namespace Contrive.Auth
{
  [Serializable]
  public class CreateUserException : Exception
  {
    public CreateUserException(UserCreateStatus statusCode) : base(GetMessageFromStatusCode(statusCode))
    {
      _statusCode = statusCode;
    }

    public CreateUserException(string message) : base(message) { }

    protected CreateUserException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
      _statusCode = (UserCreateStatus) info.GetInt32("_StatusCode");
    }

    public CreateUserException() { }

    public CreateUserException(string message, Exception innerException) : base(message, innerException) { }

    readonly UserCreateStatus _statusCode = UserCreateStatus.ProviderError;

    public UserCreateStatus StatusCode
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get { return _statusCode; }
    }

    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("_StatusCode", _statusCode);
    }

    internal static string GetMessageFromStatusCode(UserCreateStatus statusCode)
    {
      switch (statusCode)
      {
        case UserCreateStatus.Success:
          return ApplicationServicesStrings.UserManagement_no_error;
        case UserCreateStatus.InvalidUserName:
          return ApplicationServicesStrings.UserManagement_InvalidUserName;
        case UserCreateStatus.InvalidPassword:
          return ApplicationServicesStrings.UserManagement_InvalidPassword;
        case UserCreateStatus.InvalidQuestion:
          return ApplicationServicesStrings.UserManagement_InvalidQuestion;
        case UserCreateStatus.InvalidAnswer:
          return ApplicationServicesStrings.UserManagement_InvalidAnswer;
        case UserCreateStatus.InvalidEmail:
          return ApplicationServicesStrings.UserManagement_InvalidEmail;
        case UserCreateStatus.DuplicateUserName:
          return ApplicationServicesStrings.UserManagement_DuplicateUserName;
        case UserCreateStatus.DuplicateEmail:
          return ApplicationServicesStrings.UserManagement_DuplicateEmail;
        case UserCreateStatus.UserRejected:
          return ApplicationServicesStrings.UserManagement_UserRejected;
        case UserCreateStatus.InvalidProviderUserKey:
          return ApplicationServicesStrings.UserManagement_InvalidProviderUserKey;
        case UserCreateStatus.DuplicateProviderUserKey:
          return ApplicationServicesStrings.UserManagement_DuplicateProviderUserKey;
        default:
          return ApplicationServicesStrings.Service_Error;
      }
    }
  }
}