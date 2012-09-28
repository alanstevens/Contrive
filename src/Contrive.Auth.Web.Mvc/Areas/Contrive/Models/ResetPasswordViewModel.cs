using System.ComponentModel.DataAnnotations;

namespace Contrive.Auth.Web.Mvc.Areas.Contrive.Models
{
  public class ResetPasswordViewModel
  {
    [Required(ErrorMessage = "Email or UserName is required.")]
    public string EmailOrUserName { get; set; }

    //public string PasswordAnswer { get; set; }

    //public bool RequiresQuestionAndAnswer { get; set; }
  }
}