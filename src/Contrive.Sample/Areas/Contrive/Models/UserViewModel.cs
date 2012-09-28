using System.Collections.Generic;
using Contrive.Core;

namespace Contrive.Sample.Areas.Contrive.Models
{
  public class UserViewModel
  {
    public IUser User { get; set; }
    //public bool RequiresSecretQuestionAndAnswer { get; set; }
    public IEnumerable<IRole> Roles { get; set; }
  }
}