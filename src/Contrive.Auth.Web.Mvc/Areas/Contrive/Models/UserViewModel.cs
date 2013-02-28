using System.Collections.Generic;
using Contrive.Auth.Membership;

namespace Contrive.Auth.Web.Mvc.Areas.Contrive.Models
{
  public class UserViewModel
  {
    public IUserExtended User { get; set; }
    //public bool RequiresSecretQuestionAndAnswer { get; set; }
    public IEnumerable<IRole> Roles { get; set; }
  }
}