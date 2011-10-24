namespace Contrive.Core
{
  public static class AccountValidation
  {
    public static string ErrorCodeToString(UserCreateStatus createStatus)
    {
      // See http://go.microsoft.com/fwlink/?LinkID=177550 for
      // a full list of status codes.
      // TODO: HAS 09/25/2011 Move these strings out into localizable resources.
      switch (createStatus)
      {
        case UserCreateStatus.DuplicateUserName:
          return "Username already exists. Please enter a different user name.";

        case UserCreateStatus.DuplicateEmail:
          return "A userName for that e-mail address already exists. Please enter a different e-mail address.";

        case UserCreateStatus.InvalidPassword:
          return "The password provided is invalid. Please enter a valid password value.";

        case UserCreateStatus.InvalidEmail:
          return "The e-mail address provided is invalid. Please check the value and try again.";

        case UserCreateStatus.InvalidAnswer:
          return "The password retrieval answer provided is invalid. Please check the value and try again.";

        case UserCreateStatus.InvalidQuestion:
          return "The password retrieval question provided is invalid. Please check the value and try again.";

        case UserCreateStatus.InvalidUserName:
          return "The user name provided is invalid. Please check the value and try again.";

        case UserCreateStatus.ProviderError:
          return
            "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        case UserCreateStatus.UserRejected:
          return
            "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
        default:
          return
            "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
      }
    }
  }
}