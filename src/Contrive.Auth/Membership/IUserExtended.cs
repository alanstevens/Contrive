using System;
using System.Collections.Generic;

namespace Contrive.Auth.Membership
{
    public interface IUserExtended : IUser
    {
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

        string TimeZone { get; set; }

        string Culture { get; set; }

        string AuthDigest { get; set; }

        bool IsLockedOut { get; set; }

        DateTime? LastLockedOutDate { get; set; }
    }
}