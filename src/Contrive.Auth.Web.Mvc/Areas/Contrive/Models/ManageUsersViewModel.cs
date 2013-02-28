using System.Collections.Generic;
using Contrive.Auth.Membership;

namespace Contrive.Auth.Web.Mvc.Areas.Contrive.Models
{
  public class ManageUsersViewModel
  {
    public IEnumerable<IUserExtended> Users { get; set; }
  }
}