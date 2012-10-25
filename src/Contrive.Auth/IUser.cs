using System;
using System.Collections.Generic;

namespace Contrive.Auth
{
  public interface IUser
  {
    Guid Id { get; set; }

    string UserName { get; set; }

    string Email { get; set; }

    string Password { get; set; }

    string PasswordSalt { get; set; }

    bool IsApproved { get; set; }

    bool IsConfirmed { get; set; }

    string ConfirmationToken { get; set; }

    int FailedPasswordAttemptCount { get; set; }

    DateTime? LastPasswordFailureDate { get; set; }

    DateTime? DateCreated { get; set; }

    DateTime? PasswordChangedDate { get; set; }

    string PasswordVerificationToken { get; set; }

    DateTime? PasswordVerificationTokenExpirationDate { get; set; }

    DateTime? FailedPasswordAttemptWindowStart { get; set; }

    ICollection<IRole> Roles { get; set; }

    DateTime? LastPasswordChangedDate { get; set; }

    DateTime? LastActivityDate { get; set; }

    string FirstName { get; set; }

    string LastName { get; set; }

    string TimeZone { get; set; }

    string Culture { get; set; }

    string AuthDigest { get; set; }

    bool IsLockedOut { get; set; }

    DateTime? LastLockedOutDate { get; set; }
  }
}