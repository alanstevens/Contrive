using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Contrive.Auth.Membership;

namespace Contrive.Auth.Web.Mvc.Areas.Contrive.Models
{
    public class PasswordResetModel
    {
        public IUserExtended User { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public int MinRequiredNonAlphanumericCharacters { get; set; }

        public int MinRequiredPasswordLength { get; set; }
    }
}