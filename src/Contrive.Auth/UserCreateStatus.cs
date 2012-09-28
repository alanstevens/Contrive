namespace Contrive.Auth
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