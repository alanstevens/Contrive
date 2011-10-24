namespace Contrive.Core
{
  public enum UserCreateStatus
  {
    Success,
    InvalidUserName,
    InvalidPassword,
    InvalidQuestion,
    InvalidAnswer,
    InvalidEmail,
    DuplicateUserName,
    DuplicateEmail,
    UserRejected,
    InvalidProviderUserKey,
    DuplicateProviderUserKey,
    ProviderError,
  }
}