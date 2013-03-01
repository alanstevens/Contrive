using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Contrive.Auth.Membership
{
    public class UserExtended : IUserExtended
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string PasswordSalt { get; set; }

        public bool IsApproved { get; set; }
        public bool IsConfirmed { get; set; }
        public string ConfirmationToken { get; set; }
        public int FailedPasswordAttemptCount { get; set; }
        public DateTime? LastPasswordFailureDate { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? PasswordChangedDate { get; set; }
        public string PasswordVerificationToken { get; set; }
        public DateTime? PasswordVerificationTokenExpirationDate { get; set; }

        public DateTime? FailedPasswordAttemptWindowStart { get; set; }

        public ICollection<IRole> Roles { get; set; }

        //[MaxLength(100)]
        //public string PasswordQuestion { get; set; }

        //[MaxLength(100)]
        //public string PasswordAnswer { get; set; }
        //public int FailedPasswordAnswerAttemptCount { get; set; }
        //public DateTime? FailedPasswordAnswerAttemptWindowStart { get; set; }

        public DateTime? LastPasswordChangedDate { get; set; }
        public DateTime? LastActivityDate { get; set; }

        public bool IsLockedOut { get; set; }
        public DateTime? LastLockedOutDate { get; set; }

        //Optional
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TimeZone { get; set; }
        public string Culture { get; set; }

        public string AuthDigest { get; set; }
    }
}