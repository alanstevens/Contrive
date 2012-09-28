using System.Collections.Generic;

namespace Contrive.Auth.Web.Mvc.Areas.Contrive.Models
{
  public class ManageUsersViewModel
  {
    public IEnumerable<IUser> Users { get; set; }
  }
}