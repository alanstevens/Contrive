using System.Collections.Generic;

namespace Contrive.Auth.Web.Mvc.Areas.Contrive.Models
{
  public class UserViewModel
  {
    public IUser User { get; set; }
    //public bool RequiresSecretQuestionAndAnswer { get; set; }
    public IEnumerable<IRole> Roles { get; set; }
  }
}