using System.ComponentModel.DataAnnotations;

namespace Contrive.Sample.Areas.Contrive.Models
{
  public class RoleViewModel
  {
    [Required(ErrorMessage = "RoleName is required.")]
    public string RoleName { get; set; }

    public string Description { get; set; }
  }
}